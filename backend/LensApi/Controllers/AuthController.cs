using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LensApi.Dtos;
using LensApi.Models;
using LensApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LensApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        // Validate input
        InputValidator.ValidateNotEmpty(request?.Username, "username");
        InputValidator.ValidateNotEmpty(request?.Password, "password");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request!.Username);
        if (user == null || user.PasswordHash != request!.Password)
        {
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse("Invalid username or password."));
        }

        var token = BuildToken(user.Id, user.Username, user.Mode);

        // create and persist refresh token
        var refreshToken = GenerateRefreshToken();
        var refreshHash = HashToken(refreshToken);
        var refreshEntity = new Models.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        _context.RefreshTokens.Add(refreshEntity);
        await _context.SaveChangesAsync();

        // set httpOnly cookie for refresh token
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.None,
            Expires = refreshEntity.ExpiresAt
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

        var response = new AuthResponse(token, null, user.Username, user.Mode);
        return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "Login successful"));
    }

    [HttpPost("guest")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> GuestLogin()
    {
        var userId = Guid.NewGuid().ToString();
        var guestUsername = $"guest_{userId[..8]}";

        // Persist guest user to satisfy refresh_tokens foreign key.
        var guestUser = new User
        {
            Id = userId,
            Username = guestUsername,
            PasswordHash = string.Empty,
            Mode = "guest",
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(guestUser);
        // MUST save guest user first so FK constraint is satisfied
        await _context.SaveChangesAsync();

        var token = BuildToken(userId, "guest", "guest");

        var refreshToken = GenerateRefreshToken();
        var refreshHash = HashToken(refreshToken);
        var refreshEntity = new Models.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = refreshHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        _context.RefreshTokens.Add(refreshEntity);
        await _context.SaveChangesAsync();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.None,
            Expires = refreshEntity.ExpiresAt
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

        var response = new AuthResponse(token, null, "guest", "guest");
        return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "Guest login successful"));
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        InputValidator.ValidateNotEmpty(request?.Username, "username");
        InputValidator.ValidateLength(request!.Username, "username", 3, 32);
        InputValidator.ValidateNotEmpty(request.Password, "password");
        InputValidator.ValidateLength(request.Password, "password", 6, 64);

        var existing = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if (existing)
        {
            return Conflict(ApiResponse<AuthResponse>.ErrorResponse("Username already exists"));
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = request.Username,
            PasswordHash = request.Password,
            Mode = "user",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = BuildToken(user.Id, user.Username, user.Mode);

        var refreshToken = GenerateRefreshToken();
        var refreshHash = HashToken(refreshToken);
        var refreshEntity = new Models.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        _context.RefreshTokens.Add(refreshEntity);
        await _context.SaveChangesAsync();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.None,
            Expires = refreshEntity.ExpiresAt
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

        var response = new AuthResponse(token, null, user.Username, user.Mode);
        return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "Registration successful"));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileResponse>> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var mode = User.FindFirstValue(ClaimTypes.Role) ?? "user";
        if (mode == "guest")
        {
            return Ok(new UserProfileResponse("guest", "guest"));
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserProfileResponse(user.Username, user.Mode));
    }

    private string BuildToken(string userId, string username, string mode)
    {
        var key = _configuration["Jwt:Key"] ?? "dev-super-secret-key-change-me";
        var issuer = _configuration["Jwt:Issuer"] ?? "lens-app";
        var audience = _configuration["Jwt:Audience"] ?? "lens-app-users";
        var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var parsed) ? parsed : 15;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(ClaimTypes.Role, mode)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request)
    {
        // Read incoming token from body or cookie
        var incoming = request.RefreshToken;
        if (string.IsNullOrWhiteSpace(incoming)) incoming = Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(incoming)) return BadRequest(new { message = "Refresh token is required." });

        var hash = HashToken(incoming);
        var existing = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash);
        if (existing == null)
        {
            Console.WriteLine($"[SECURITY] Unknown refresh token presented (hash: {hash.Substring(0,8)}...)\n");
            return Unauthorized(new { message = "Invalid refresh token." });
        }

        // If token is active -> rotate normally
        if (existing.IsActive)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == existing.UserId);
            var username = user?.Username ?? "guest";
            var mode = user?.Mode ?? "guest";
            var accessToken = BuildToken(existing.UserId, username, mode);

            // rotate refresh token
            var newRefresh = GenerateRefreshToken();
            var newHash = HashToken(newRefresh);
            existing.RevokedAt = DateTime.UtcNow;
            existing.ReplacedByTokenHash = newHash;
            var newEntity = new Models.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = existing.UserId,
                TokenHash = newHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            _context.RefreshTokens.Add(newEntity);
            await _context.SaveChangesAsync();

            // set new refresh cookie (httpOnly)
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.None,
                Expires = newEntity.ExpiresAt
            };
            Response.Cookies.Append("refreshToken", newRefresh, cookieOptions);

            return Ok(new AuthResponse(accessToken, null, username, mode));
        }

        // Token is not active -> possible reuse/compromise
        Console.WriteLine($"[SECURITY] Inactive refresh token used for user {existing.UserId} (revoked_at={existing.RevokedAt}). Treating as reuse.");
        // Revoke all other active tokens for this user
        var userTokens = await _context.RefreshTokens.Where(t => t.UserId == existing.UserId && t.RevokedAt == null).ToListAsync();
        foreach (var t in userTokens) t.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // delete cookie
        Response.Cookies.Delete("refreshToken");

        return Unauthorized(new { message = "Refresh token reuse detected. All sessions revoked." });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshRequest request)
    {
        var incoming = request.RefreshToken;
        if (string.IsNullOrWhiteSpace(incoming)) incoming = Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(incoming)) return BadRequest();
        var hash = HashToken(incoming);
        var existing = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash);
        if (existing == null) return NotFound();
        existing.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // remove cookie
        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<ActionResult<List<SessionResponse>>> GetSessions()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token" });
        }

        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var sessions = tokens.Select(t => new SessionResponse(
            t.Id,
            t.CreatedAt,
            t.ExpiresAt,
            t.IsActive,
            t.RevokedAt
        )).ToList();

        return Ok(sessions);
    }

    [Authorize]
    [HttpDelete("sessions/{tokenId}")]
    public async Task<IActionResult> RevokeSession(Guid tokenId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Id == tokenId && t.UserId == userId);
        if (token == null) return NotFound();

        token.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // if revoking current session, delete cookie
        var incoming = Request.Cookies["refreshToken"];
        if (!string.IsNullOrWhiteSpace(incoming) && HashToken(incoming) == token.TokenHash)
        {
            Response.Cookies.Delete("refreshToken");
        }

        return NoContent();
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    public record LoginRequest(string Username, string Password);
    public record RegisterRequest(string Username, string Password);
    public record RefreshRequest(string RefreshToken);
    public record AuthResponse(string Token, string? RefreshToken, string Username, string Mode);
    public record UserProfileResponse(string Username, string Mode);
    public record SessionResponse(Guid TokenId, DateTime CreatedAt, DateTime ExpiresAt, bool IsActive, DateTime? RevokedAt);
}
