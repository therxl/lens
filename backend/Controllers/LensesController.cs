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
        return Ok(_lensRepository.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Lens> GetLens(int id)
    {
        var lens = _lensRepository.GetById(id);
        if (lens == null)
        {
            return NotFound();
        }
        return Ok(lens);
    }
}