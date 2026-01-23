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
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("UserId is required");
        }
        return Ok(_favoritesRepository.GetFavorites(userId));
    }

    [HttpPost]
    public ActionResult AddToFavorites([FromQuery] string userId, [FromBody] Lens lens)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("UserId is required");
        }
        if (lens == null || lens.Id == 0)
        {
            return BadRequest("Lens is required");
        }
        // Optionally validate lens exists
        var existingLens = _lensRepository.GetById(lens.Id);
        if (existingLens == null)
        {
            return NotFound("Lens not found");
        }
        var added = _favoritesRepository.AddToFavorites(userId, lens);
        if (!added)
        {
            return Conflict("Lens already in favorites");
        }
        return Ok();
    }

    [HttpDelete("{lensId}")]
    public ActionResult RemoveFromFavorites(int lensId, [FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("UserId is required");
        }
        var removed = _favoritesRepository.RemoveFromFavorites(userId, lensId);
        if (!removed)
        {
            return NotFound("Lens not in favorites");
        }
        return Ok();
    }
}