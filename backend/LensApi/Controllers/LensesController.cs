using LensApi.Dtos;
using LensApi.Exceptions;
using LensApi.Models;
using LensApi.Repositories;
using LensApi.Validators;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LensApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LensesController : ControllerBase
{
    private readonly ILensRepository _lensRepository;
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions CacheJsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
    };

    public LensesController(ILensRepository lensRepository, IDistributedCache cache)
    {
        _lensRepository = lensRepository;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Lens>>>> GetLenses(
        [FromQuery] string? search,
        [FromQuery] string? type,
        [FromQuery] string? brand,
        [FromQuery] int? minFocal,
        [FromQuery] int? maxFocal,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sort)
    {
        try
        {
            var query = await BuildLensQueryAsync(search, type, brand, minFocal, maxFocal, minPrice, maxPrice, sort);
            return Ok(ApiResponse<IEnumerable<Lens>>.SuccessResponse(query.ToList()));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Lenses GET] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to fetch lenses"));
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Lens>>>> Search(
        [FromQuery] string? search,
        [FromQuery] string? type,
        [FromQuery] string? brand,
        [FromQuery] int? minFocal,
        [FromQuery] int? maxFocal,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sort)
    {
        try
        {
            var query = await BuildLensQueryAsync(search, type, brand, minFocal, maxFocal, minPrice, maxPrice, sort);
            return Ok(ApiResponse<IEnumerable<Lens>>.SuccessResponse(query.ToList()));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Lenses SEARCH] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to search lenses"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Lens>>> GetLens(int id)
    {
        try
        {
            InputValidator.ValidatePositiveId(id, "id");

            var lens = (await GetCatalogAsync()).FirstOrDefault(l => l.Id == id);
            if (lens == null)
            {
                return NotFound(ApiResponse<Lens>.ErrorResponse($"Lens with id {id} not found"));
            }
            return Ok(ApiResponse<Lens>.SuccessResponse(lens));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Lenses GET by ID] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to fetch lens"));
        }
    }

    [HttpGet("popular")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Lens>>>> GetPopularLenses()
    {
        try
        {
            var lenses = (await GetCatalogAsync()).Where(l => l.IsPopular == true);
            return Ok(ApiResponse<IEnumerable<Lens>>.SuccessResponse(lenses));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Lenses GET popular] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to fetch popular lenses"));
        }
    }

    [HttpGet("brand/{brand}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Lens>>>> GetLensesByBrand(string brand)
    {
        try
        {
            InputValidator.ValidateNotEmpty(brand, "brand");

            var lenses = (await GetCatalogAsync()).Where(l => l.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase));
            return Ok(ApiResponse<IEnumerable<Lens>>.SuccessResponse(lenses));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Lenses GET by brand] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to fetch lenses"));
        }
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Lens>>>> GetLensesByType(string type)
    {
        try
        {
            InputValidator.ValidateNotEmpty(type, "type");

            var lenses = (await GetCatalogAsync()).Where(l => l.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
            return Ok(ApiResponse<IEnumerable<Lens>>.SuccessResponse(lenses));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse.ValidationErrorResponse(ex.Errors));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Lenses GET by type] Error: {ex.Message}");
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to fetch lenses"));
        }
    }

    private async Task<List<Lens>> GetCatalogAsync()
    {
        const string cacheKey = "lenses:catalog:all";

        try
        {
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                var cachedCatalog = JsonSerializer.Deserialize<List<Lens>>(cached, CacheJsonOptions);
                if (cachedCatalog != null)
                {
                    return cachedCatalog;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Cache] Lens catalog read skipped: {ex.Message}");
        }

        var lenses = _lensRepository.GetAllLenses().ToList();

        try
        {
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(lenses, CacheJsonOptions), CacheOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Cache] Lens catalog write skipped: {ex.Message}");
        }

        return lenses;
    }

    private async Task<IEnumerable<Lens>> BuildLensQueryAsync(
        string? search,
        string? type,
        string? brand,
        int? minFocal,
        int? maxFocal,
        decimal? minPrice,
        decimal? maxPrice,
        string? sort)
    {
        if (minFocal.HasValue && maxFocal.HasValue && minFocal > maxFocal)
        {
            var errors = new Dictionary<string, string[]>
            {
                ["minFocal"] = new[] { "minFocal must be <= maxFocal" },
                ["maxFocal"] = new[] { "maxFocal must be >= minFocal" }
            };
            throw new ValidationException(errors);
        }

        if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
        {
            var errors = new Dictionary<string, string[]>
            {
                ["minPrice"] = new[] { "minPrice must be <= maxPrice" },
                ["maxPrice"] = new[] { "maxPrice must be >= minPrice" }
            };
            throw new ValidationException(errors);
        }

        var lenses = await GetCatalogAsync();
        IEnumerable<Lens> query = lenses;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(l =>
                l.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                l.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(l => l.Type.Equals(type.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(l => l.Brand.Equals(brand.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (minFocal.HasValue)
        {
            query = query.Where(l => l.MaxFocal >= minFocal.Value);
        }

        if (maxFocal.HasValue)
        {
            query = query.Where(l => l.MinFocal <= maxFocal.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(l => l.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(l => l.Price <= maxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(sort))
        {
            query = sort.Trim().ToLowerInvariant() switch
            {
                "price_asc" => query.OrderBy(l => l.Price),
                "price_desc" => query.OrderByDescending(l => l.Price),
                "focal_asc" => query.OrderBy(l => l.MinFocal),
                "focal_desc" => query.OrderByDescending(l => l.MinFocal),
                "name_asc" => query.OrderBy(l => l.Name),
                "name_desc" => query.OrderByDescending(l => l.Name),
                _ => throw new ValidationException("sort", "sort must be one of: price_asc, price_desc, focal_asc, focal_desc, name_asc, name_desc")
            };
        }

        return query;
    }
}