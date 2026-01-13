using LensApi.Models;
using LensApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LensApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LensesController : ControllerBase
{
    private readonly ILensRepository _lensRepository;

    public LensesController(ILensRepository lensRepository)
    {
        _lensRepository = lensRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Lens>> GetLenses()
    {
        var lenses = _lensRepository.GetAllLenses();
        return Ok(lenses);
    }

    [HttpGet("{id}")]
    public ActionResult<Lens> GetLens(int id)
    {
        var lens = _lensRepository.GetLensById(id);
        if (lens == null)
        {
            return NotFound();
        }
        return Ok(lens);
    }
<<<<<<< HEAD
=======

    [HttpGet("popular")]
    public ActionResult<IEnumerable<Lens>> GetPopularLenses()
    {
        var lenses = _lensRepository.GetPopularLenses();
        return Ok(lenses);
    }

    [HttpGet("brand/{brand}")]
    public ActionResult<IEnumerable<Lens>> GetLensesByBrand(string brand)
    {
        var lenses = _lensRepository.GetLensesByBrand(brand);
        return Ok(lenses);
    }

    [HttpGet("type/{type}")]
    public ActionResult<IEnumerable<Lens>> GetLensesByType(string type)
    {
        var lenses = _lensRepository.GetLensesByType(type);
        return Ok(lenses);
    }
>>>>>>> 042e6a4 (lab4)
}