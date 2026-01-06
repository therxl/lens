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
}