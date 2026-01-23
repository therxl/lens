using LensBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LensBackend.Repositories;

public class FavoritesRepository
{
    private readonly ApplicationDbContext _context;

    public FavoritesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Используем raw SQL для получения избранных линз
    public IEnumerable<Lens> GetFavorites(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
        }
        var sql = "SELECT l.* FROM \"Lenses\" l INNER JOIN \"Favorites\" f ON l.\"Id\" = f.\"LensId\" WHERE f.\"UserId\" = {0}";
        return _context.Lenses.FromSqlRaw(sql, userId).ToList();
    }

    // Используем EF для добавления в избранное
    public async Task<bool> AddToFavorites(string userId, int lensId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
        }
        if (lensId <= 0)
        {
            throw new ArgumentException("LensId must be positive", nameof(lensId));
        }
        var existing = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.LensId == lensId);
        if (existing != null)
        {
            return false; // Already in favorites
        }
        var favorite = new Favorite { UserId = userId, LensId = lensId };
        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    // Используем EF для удаления из избранного
    public async Task<bool> RemoveFromFavorites(string userId, int lensId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
        }
        if (lensId <= 0)
        {
            throw new ArgumentException("LensId must be positive", nameof(lensId));
        }
        var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.LensId == lensId);
        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}