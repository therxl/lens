using System.IdentityModel.Tokens.Jwt;
using LensApi.Dtos;
using LensApi.Exceptions;
using LensApi.Models;
using LensApi.Messaging;
using LensApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LensApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly RabbitMqCacheInvalidationPublisher _cacheInvalidationPublisher;

    public FavoritesController(ApplicationDbContext context, RabbitMqCacheInvalidationPublisher cacheInvalidationPublisher)
    {
        _context = context;
        _cacheInvalidationPublisher = cacheInvalidationPublisher;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Lens>>>> GetFavorites(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 50)
            {
                var errors = new Dictionary<string, string[]>();
                if (page < 1) errors["page"] = new[] { "page must be >= 1" };
                if (pageSize < 1 || pageSize > 50) errors["pageSize"] = new[] { "pageSize must be between 1 and 50" };
                return BadRequest(ApiResponse.ValidationErrorResponse(errors));
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(ApiResponse.ErrorResponse("User not found in token"));
            }

            var mode = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (mode == "guest")
            {
                return Ok(ApiResponse<IEnumerable<Lens>>.SuccessResponse(new List<Lens>()));
            }

            var lenses = await _context.Favorites
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Join(_context.Lenses,
                    favorite => favorite.LensId,
                    lens => lens.Id,
                    (fav, lens) => lens)
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<Lens>>.SuccessResponse(lenses));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Favorites GET] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to fetch favorites"));
        }
    }

    [HttpPost("{lensId:int}")]
    public async Task<ActionResult<ApiResponse>> AddFavorite(int lensId)
    {
        try
        {
            InputValidator.ValidatePositiveId(lensId, "lensId");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(ApiResponse.ErrorResponse("User not found in token"));
            }

            var mode = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (mode == "guest")
            {
                return BadRequest(ApiResponse.ErrorResponse("Guest user cannot store favorites on server"));
            }

            var lensExists = await _context.Lenses.AnyAsync(l => l.Id == lensId);
            if (!lensExists)
            {
                return NotFound(ApiResponse.ErrorResponse($"Lens with id {lensId} not found"));
            }

            var exists = await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.LensId == lensId);
            if (exists)
            {
                return Conflict(ApiResponse.ErrorResponse("Lens is already in your favorites"));
            }

            _context.Favorites.Add(new Favorite
            {
                UserId = userId,
                LensId = lensId,
                AddedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            await _cacheInvalidationPublisher.PublishAsync(new CacheInvalidationMessage(
                Reason: "favorites-updated",
                CacheKeys: CacheInvalidationTargets.RecommendationKeys,
                UserId: userId,
                LensId: lensId,
                OccurredAtUtc: DateTimeOffset.UtcNow));
            
            Console.WriteLine($"[Favorites ADD] User {userId} added lens {lensId} to favorites");
            return Ok(ApiResponse.SuccessResponse("Lens added to favorites successfully"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
        {
            return Conflict(ApiResponse.ErrorResponse("Lens is already in your favorites"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Favorites ADD] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to add favorite"));
        }
    }

    [HttpDelete("{lensId:int}")]
    public async Task<ActionResult<ApiResponse>> RemoveFavorite(int lensId)
    {
        try
        {
            InputValidator.ValidatePositiveId(lensId, "lensId");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(ApiResponse.ErrorResponse("User not found in token"));
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.LensId == lensId);

            if (favorite == null)
            {
                return NotFound(ApiResponse.ErrorResponse($"Lens with id {lensId} not found in your favorites"));
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            await _cacheInvalidationPublisher.PublishAsync(new CacheInvalidationMessage(
                Reason: "favorites-updated",
                CacheKeys: CacheInvalidationTargets.RecommendationKeys,
                UserId: userId,
                LensId: lensId,
                OccurredAtUtc: DateTimeOffset.UtcNow));

            Console.WriteLine($"[Favorites DELETE] User {userId} removed lens {lensId} from favorites");
            return Ok(ApiResponse.SuccessResponse("Lens removed from favorites successfully"));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Favorites DELETE] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to remove favorite"));
        }
    }
}
