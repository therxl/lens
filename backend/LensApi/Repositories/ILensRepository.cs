using LensApi.Models;

namespace LensApi.Repositories;

public interface ILensRepository
{
    IEnumerable<Lens> GetAllLenses();
    Lens? GetLensById(int id);
    IEnumerable<Lens> GetPopularLenses();
    IEnumerable<Lens> GetLensesByBrand(string brand);
    IEnumerable<Lens> GetLensesByType(string type);
}