using LensBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LensBackend.Repositories;

public class LensRepository
{
    private readonly ApplicationDbContext _context;

    public LensRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Lens> GetAll() => _context.Lenses.FromSqlRaw("SELECT * FROM \"Lenses\"").ToList();

    public Lens? GetById(int id) => _context.Lenses.FromSqlRaw("SELECT * FROM \"Lenses\" WHERE \"Id\" = {0}", id).FirstOrDefault();
}