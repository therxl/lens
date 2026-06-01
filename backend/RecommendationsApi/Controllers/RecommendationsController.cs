using System.Security.Claims;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RecommendationsApi.Data;
using RecommendationsApi.Dtos;
using RecommendationsApi.Models;
using RecommendationsApi.Validators;
using System.Text.Json;

namespace RecommendationsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecommendationsController : ControllerBase
{
    private readonly RecommendationsDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDistributedCache _cache;
    private static readonly List<ShootingTypeProfile> ShootingProfiles = RecommendationsDbContext.GetShootingTypeProfiles();
    private static readonly JsonSerializerOptions CacheJsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    };

    public RecommendationsController(RecommendationsDbContext context, IHttpClientFactory httpClientFactory, IDistributedCache cache)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _cache = cache;
    }

    /// <summary>
    /// Получить рекомендации объективов по типу съемки
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RecommendationResponse>>> GetRecommendations([FromBody] GetRecommendationsRequest request)
    {
        try
        {
            InputValidator.ValidateNotEmpty(request?.ShootingType, "shootingType");

            var shootingType = request!.ShootingType.ToLower();
            var profile = ShootingProfiles.FirstOrDefault(p => p.Type == shootingType);
            
            if (profile == null)
            {
                var validTypes = string.Join(", ", ShootingProfiles.Select(p => p.Type));
                throw new Exceptions.ValidationException("shootingType", $"Unknown shooting type. Valid types: {validTypes}");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(ApiResponse<RecommendationResponse>.ErrorResponse("User not found in token"));
            }

            var recommendations = await GetRecommendationsFromLensApi(profile);
            
            // Записываем в историю, что пользователь просмотрел рекомендации
            _context.SelectionHistories.Add(new SelectionHistory
            {
                UserId = userId,
                ShootingType = shootingType,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            Console.WriteLine($"[Recommendations] User {userId} requested recommendations for {shootingType}");

            var response = new RecommendationResponse
            {
                ShootingType = shootingType,
                Description = profile.Description,
                Recommendations = recommendations
            };

            return Ok(ApiResponse<RecommendationResponse>.SuccessResponse(response));
        }
        catch (Exceptions.ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Recommendations] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to get recommendations"));
        }
    }

    /// <summary>
    /// Записать выбор пользователя (выбор объектива)
    /// </summary>
    [HttpPost("record")]
    public async Task<ActionResult<ApiResponse>> RecordSelection([FromBody] RecordSelectionRequest request)
    {
        try
        {
            InputValidator.ValidateNotEmpty(request?.ShootingType, "shootingType");
            var shootingType = request!.ShootingType.ToLower();
            if (request.LensId < 1)
            {
                throw new Exceptions.ValidationException("lensId", "lensId must be a positive number");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(ApiResponse.ErrorResponse("User not found in token"));
            }

            var mode = User.FindFirst(ClaimTypes.Role)?.Value;
            if (mode == "guest")
            {
                return BadRequest(ApiResponse.ErrorResponse("Guest users cannot save recommendations"));
            }

            _context.SelectionHistories.Add(new SelectionHistory
            {
                UserId = userId,
                ShootingType = shootingType,
                SelectedLensId = request.LensId,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            Console.WriteLine($"[Recommendations] User {userId} selected lens {request.LensId} for {shootingType}");

            return Ok(ApiResponse.SuccessResponse("Selection recorded successfully"));
        }
        catch (Exceptions.ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Recommendations] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to record selection"));
        }
    }

    /// <summary>
    /// Получить историю выборов пользователя
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SelectionHistoryResponse>>>> GetHistory(
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

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(ApiResponse.ErrorResponse("User not found in token"));
            }

            var history = await _context.SelectionHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new SelectionHistoryResponse
                {
                    Id = h.Id,
                    ShootingType = h.ShootingType,
                    SelectedLensId = h.SelectedLensId,
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync();

            Console.WriteLine($"[Recommendations] User {userId} retrieved history ({history.Count} records)");
            return Ok(ApiResponse<IEnumerable<SelectionHistoryResponse>>.SuccessResponse(history));
        }
        catch (Exceptions.ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Recommendations] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to get history"));
        }
    }

    /// <summary>
    /// Получить статистику выборов пользователя
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<UserRecommendationStats>>> GetStats()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(ApiResponse<UserRecommendationStats>.ErrorResponse("User not found in token"));
            }

            var history = await _context.SelectionHistories
                .Where(h => h.UserId == userId)
                .ToListAsync();

            var stats = new UserRecommendationStats
            {
                TotalSelections = history.Count,
                UniqueShootingTypes = history
                    .Select(h => h.ShootingType)
                    .Distinct()
                    .ToList(),
                ShootingTypeDistribution = history
                    .GroupBy(h => h.ShootingType)
                    .Select(g => new ShootingTypeCount { ShootingType = g.Key, Count = g.Count() })
                    .ToList(),
                MostSelectedLensId = history
                    .Where(h => h.SelectedLensId.HasValue)
                    .GroupBy(h => h.SelectedLensId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault()
            };

            Console.WriteLine($"[Recommendations] User {userId} retrieved stats");
            return Ok(ApiResponse<UserRecommendationStats>.SuccessResponse(stats));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Recommendations] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to get stats"));
        }
    }

    private async Task<List<RecommendedLens>> GetRecommendationsFromLensApi(ShootingTypeProfile profile)
    {
        var cacheKey = $"recommendations:{profile.Type}";

        try
        {
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                var cachedRecommendations = JsonSerializer.Deserialize<List<RecommendedLens>>(cached, CacheJsonOptions);
                if (cachedRecommendations != null)
                {
                    return cachedRecommendations;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Cache] Recommendations read skipped: {ex.Message}");
        }

        var httpClient = _httpClientFactory.CreateClient("LensApi");
        var lensResponse = await httpClient.GetFromJsonAsync<ApiResponse<List<LensCatalogItem>>>("api/lenses");
        if (lensResponse == null)
        {
            throw new Exception("LensApi returned empty response");
        }

        if (!lensResponse.Success)
        {
            throw new Exception(lensResponse.Message ?? "LensApi returned an error");
        }

        var lenses = lensResponse.Data ?? [];

        var recommendations = lenses
            .Select(lens => BuildRecommendation(lens, profile))
            .Where(recommendation => recommendation != null)
            .Select(recommendation => recommendation!)
            .OrderByDescending(recommendation => recommendation.MatchScore)
            .ThenByDescending(recommendation => recommendation.Price)
            .Take(4)
            .Select(recommendation => new RecommendedLens
            {
                Id = recommendation.Id,
                Name = recommendation.Name,
                Type = recommendation.Type,
                FocalLength = recommendation.FocalLength,
                Aperture = recommendation.Aperture,
                Brand = recommendation.Brand,
                Price = recommendation.Price,
                ImageUrl = CreateLensArtwork(recommendation.Name, recommendation.Brand, recommendation.FocalLength, recommendation.AccentColor),
                MatchReason = recommendation.MatchReason
            })
            .ToList();

        try
        {
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(recommendations, CacheJsonOptions), CacheOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Cache] Recommendations write skipped: {ex.Message}");
        }

        return recommendations;
    }

    private static RecommendationCandidate? BuildRecommendation(LensCatalogItem lens, ShootingTypeProfile profile)
    {
        var typeScore = lens.Type.Equals(profile.Type, StringComparison.OrdinalIgnoreCase) ? 40 : 0;
        var brandScore = profile.PreferredBrands.Contains(lens.Brand) ? 20 : 0;
        var focalScore = GetFocalScore(lens.MinFocal, lens.MaxFocal, profile);
        var apertureScore = GetApertureScore(lens.Aperture, profile);

        var score = typeScore + brandScore + focalScore + apertureScore;
        if (score <= 0)
        {
            return null;
        }

        var accentColor = GetAccentColor(lens.Type);

        return new RecommendationCandidate
        {
            Id = lens.Id,
            Name = lens.Name,
            Type = lens.Type,
            FocalLength = lens.FocalLength,
            Aperture = lens.Aperture,
            Brand = lens.Brand,
            Price = lens.Price,
            MatchReason = BuildMatchReason(lens, profile),
            AccentColor = accentColor,
            MatchScore = score
        };
    }

    private static int GetFocalScore(int minFocal, int maxFocal, ShootingTypeProfile profile)
    {
        if (profile.PreferredFocalMin == null || profile.PreferredFocalMax == null)
        {
            return 0;
        }

        var center = (minFocal + maxFocal) / 2;
        var targetCenter = (profile.PreferredFocalMin.Value + profile.PreferredFocalMax.Value) / 2;
        var distance = Math.Abs(center - targetCenter);

        return distance switch
        {
            <= 10 => 25,
            <= 25 => 18,
            <= 50 => 10,
            _ => 0
        };
    }

    private static int GetApertureScore(string aperture, ShootingTypeProfile profile)
    {
        if (profile.PreferredApertureMin == null)
        {
            return 0;
        }

        var normalized = aperture.Replace("f/", string.Empty, StringComparison.OrdinalIgnoreCase);
        if (!decimal.TryParse(normalized, out var apertureValue))
        {
            return 0;
        }

        var threshold = profile.PreferredApertureMin.Value;
        if (apertureValue <= threshold)
        {
            return 15;
        }

        if (apertureValue <= threshold + 1)
        {
            return 8;
        }

        return 0;
    }

    private static string BuildMatchReason(LensCatalogItem lens, ShootingTypeProfile profile)
    {
        var parts = new List<string>();

        if (lens.Type.Equals(profile.Type, StringComparison.OrdinalIgnoreCase))
        {
            parts.Add("подходит по сценарию");
        }

        if (profile.PreferredBrands.Contains(lens.Brand))
        {
            parts.Add($"бренд {lens.Brand} совпадает с профилем");
        }

        if (profile.PreferredFocalMin.HasValue && profile.PreferredFocalMax.HasValue)
        {
            parts.Add($"фокусное {lens.FocalLength} попадает в целевой диапазон");
        }

        if (profile.PreferredApertureMin.HasValue)
        {
            parts.Add($"светосила {lens.Aperture} соответствует задаче");
        }

        return parts.Count > 0 ? string.Join(", ", parts) : "Подходит под выбранный сценарий";
    }

    private static string GetAccentColor(string type)
    {
        return type.ToLowerInvariant() switch
        {
            "portrait" => "#1F1F1F",
            "landscape" => "#3A3A3A",
            "macro" => "#2B2B2B",
            "sports" => "#242424",
            _ => "#363636"
        };
    }

    private sealed class RecommendationCandidate
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Type { get; init; } = null!;
        public string FocalLength { get; init; } = null!;
        public string Aperture { get; init; } = null!;
        public string Brand { get; init; } = null!;
        public decimal Price { get; init; }
        public string MatchReason { get; init; } = null!;
        public string AccentColor { get; init; } = null!;
        public int MatchScore { get; init; }
    }

    private sealed class LensCatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string FocalLength { get; set; } = null!;
        public int MinFocal { get; set; }
        public int MaxFocal { get; set; }
        public string Aperture { get; set; } = null!;
        public string Compatibility { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public int Price { get; set; }
        public string Description { get; set; } = null!;
        public bool? IsPopular { get; set; }
    }

    private static string CreateLensArtwork(string name, string brand, string focalLength, string accentColor)
    {
        var safeName = EscapeXml(name);
        var safeBrand = EscapeXml(brand);
        var safeFocal = EscapeXml(focalLength);

        var svg = $$"""
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 960 720" role="img" aria-label="{{safeName}}">
  <defs>
    <linearGradient id="bg" x1="0" y1="0" x2="1" y2="1">
      <stop offset="0%" stop-color="#FAFAFA" />
      <stop offset="100%" stop-color="#E9E9E9" />
    </linearGradient>
    <radialGradient id="shine" cx="0.35" cy="0.25" r="0.7">
      <stop offset="0%" stop-color="#FFFFFF" stop-opacity="0.95" />
      <stop offset="100%" stop-color="#FFFFFF" stop-opacity="0" />
    </radialGradient>
  </defs>
  <rect width="960" height="720" rx="48" fill="url(#bg)" />
  <circle cx="708" cy="116" r="86" fill="#FFFFFF" fill-opacity="0.75" />
  <circle cx="228" cy="604" r="132" fill="#FFFFFF" fill-opacity="0.75" />
  <g transform="translate(480 320)">
    <circle r="210" fill="#1A1A1A" />
    <circle r="156" fill="#2A2A2A" stroke="#0B0B0B" stroke-width="18" />
    <circle r="108" fill="#0E1117" stroke="{accentColor}" stroke-width="14" />
    <circle r="48" fill="#050607" />
    <ellipse cx="-44" cy="-56" rx="66" ry="82" fill="url(#shine)" opacity="0.55" />
        <rect x="-138" y="-250" width="276" height="48" rx="24" fill="#D8D8D8" fill-opacity="0.35" />
        <rect x="-168" y="210" width="336" height="44" rx="22" fill="#D8D8D8" fill-opacity="0.35" />
  </g>
  <g fill="#111111" font-family="Arial, Helvetica, sans-serif" text-anchor="middle">
    <text x="480" y="74" font-size="22" font-weight="700" letter-spacing="4">{{safeBrand}}</text>
    <text x="480" y="628" font-size="38" font-weight="700">{{safeName}}</text>
    <text x="480" y="664" font-size="20" fill="#444444">{{safeFocal}}</text>
  </g>
</svg>
""";

        return "data:image/svg+xml;charset=utf-8," + Uri.EscapeDataString(svg);
    }

    private static string EscapeXml(string value)
    {
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}

public class GetRecommendationsRequest
{
    public string ShootingType { get; set; } = null!;
}

public class RecordSelectionRequest
{
    public string ShootingType { get; set; } = null!;
    public int LensId { get; set; }
}

public class SelectionHistoryResponse
{
    public Guid Id { get; set; }
    public string ShootingType { get; set; } = null!;
    public int? SelectedLensId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserRecommendationStats
{
    public int TotalSelections { get; set; }
    public List<string> UniqueShootingTypes { get; set; } = [];
    public List<ShootingTypeCount> ShootingTypeDistribution { get; set; } = [];
    public int? MostSelectedLensId { get; set; }
}

public class ShootingTypeCount
{
    public string ShootingType { get; set; } = null!;
    public int Count { get; set; }
}
