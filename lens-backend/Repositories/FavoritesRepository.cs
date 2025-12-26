using lens_backend.Models;

namespace lens_backend.Repositories;

public class FavoritesRepository
{
    private readonly Dictionary<string, List<Lens>> _userFavorites = new();

    public IEnumerable<Lens> GetFavorites(string userId)
    {
        if (_userFavorites.TryGetValue(userId, out var favorites))
        {
            return favorites;
        }
        return new List<Lens>();
    }

    public void AddToFavorites(string userId, Lens lens)
    {
        if (!_userFavorites.ContainsKey(userId))
        {
            _userFavorites[userId] = new List<Lens>();
        }
        if (!_userFavorites[userId].Any(f => f.Id == lens.Id))
        {
            _userFavorites[userId].Add(lens);
        }
    }

    public void RemoveFromFavorites(string userId, int lensId)
    {
        if (_userFavorites.TryGetValue(userId, out var favorites))
        {
            favorites.RemoveAll(f => f.Id == lensId);
        }
    }
}