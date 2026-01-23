using LensBackend.Models;
using LensBackend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LensBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LensesController : ControllerBase
{
    private readonly LensRepository _lensRepository;

    public LensesController(LensRepository lensRepository)
    {
        _lensRepository = lensRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Lens>> GetLenses()
    {
        try
        {
            var lenses = _lensRepository.GetAll();
            return Ok(lenses);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting lenses: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Lens> GetLens(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid id");
        }
        try
        {
            var lens = _lensRepository.GetById(id);
            if (lens == null)
            {
                return NotFound();
            }
            return Ok(lens);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting lens {id}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}