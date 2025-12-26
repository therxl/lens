using Microsoft.AspNetCore.Mvc;
using lens_backend.Models;
using lens_backend.Repositories;

namespace lens_backend.Controllers;

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
    public IActionResult AddToFavorites([FromQuery] string userId, [FromBody] int lensId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("UserId is required");
        }
        var lens = _lensRepository.GetById(lensId);
        if (lens == null)
        {
            return NotFound("Lens not found");
        }
        _favoritesRepository.AddToFavorites(userId, lens);
        return Ok();
    }

    [HttpDelete]
    public IActionResult RemoveFromFavorites([FromQuery] string userId, [FromQuery] int lensId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("UserId is required");
        }
        _favoritesRepository.RemoveFromFavorites(userId, lensId);
        return Ok();
    }
}