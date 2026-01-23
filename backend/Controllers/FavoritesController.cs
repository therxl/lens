using LensBackend.Models;
using LensBackend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LensBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly FavoritesRepository _favoritesRepository;
    private readonly LensRepository _lensRepository;

    public FavoritesController(FavoritesRepository favoritesRepository, LensRepository lensRepository)
    {
        _favoritesRepository = favoritesRepository;
        _lensRepository = lensRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Lens>> GetFavorites([FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("UserId is required and cannot be empty");
        }
        try
        {
            var favorites = _favoritesRepository.GetFavorites(userId);
            return Ok(favorites);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting favorites for user {userId}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{lensId}")]
    public async Task<ActionResult> AddToFavorites(int lensId, [FromQuery] string userId)
    {
        Console.WriteLine($"AddToFavorites: lensId={lensId}, userId='{userId}'");
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("UserId is required and cannot be empty");
        }
        if (lensId <= 0)
        {
            return BadRequest("Invalid lensId");
        }
        try
        {
            // Validate lens exists
            var existingLens = _lensRepository.GetById(lensId);
            if (existingLens == null)
            {
                return NotFound("Lens not found");
            }
            var added = await _favoritesRepository.AddToFavorites(userId, lensId);
            if (!added)
            {
                return Conflict("Lens already in favorites");
            }
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding to favorites: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{lensId}")]
    public async Task<ActionResult> RemoveFromFavorites(int lensId, [FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("UserId is required and cannot be empty");
        }
        if (lensId <= 0)
        {
            return BadRequest("Invalid lensId");
        }
        try
        {
            var removed = await _favoritesRepository.RemoveFromFavorites(userId, lensId);
            if (!removed)
            {
                return NotFound("Lens not in favorites");
            }
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing from favorites: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}