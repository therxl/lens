using LensBackend.Models;

namespace LensBackend.Repositories;

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

    public bool AddToFavorites(string userId, Lens lens)
    {
        if (!_userFavorites.ContainsKey(userId))
        {
            _userFavorites[userId] = new List<Lens>();
        }
        if (_userFavorites[userId].Any(f => f.Id == lens.Id))
        {
            return false; // Already in favorites
        }
        _userFavorites[userId].Add(lens);
        return true;
    }

    public bool RemoveFromFavorites(string userId, int lensId)
    {
        if (_userFavorites.TryGetValue(userId, out var favorites))
        {
            var lens = favorites.FirstOrDefault(f => f.Id == lensId);
            if (lens != null)
            {
                favorites.Remove(lens);
                return true;
            }
        }
        return false;
    }
}