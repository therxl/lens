using LensApi.Models;

namespace LensApi.Repositories;

public interface ILensRepository
{
    IEnumerable<Lens> GetAllLenses();
    Lens? GetLensById(int id);
}