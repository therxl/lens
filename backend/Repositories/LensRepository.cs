using LensBackend.Models;

namespace LensBackend.Repositories;

public class LensRepository
{
    private readonly List<Lens> _lenses = new()
    {
        new Lens
        {
            Id = 1,
            Name = "Canon EF 85mm f/1.8",
            Type = "portrait",
            FocalLength = "85mm",
            MinFocal = 85,
            MaxFocal = 85,
            Aperture = "f/1.8",
            Compatibility = "Canon EF",
            Brand = "Canon",
            Price = 35000,
            Description = "Классический портретный объектив.",
            IsPopular = true
        },
        new Lens
        {
            Id = 2,
            Name = "Canon EF 24-70mm f/2.8",
            Type = "landscape",
            FocalLength = "24–70mm",
            MinFocal = 24,
            MaxFocal = 70,
            Aperture = "f/2.8",
            Compatibility = "Canon EF",
            Brand = "Canon",
            Price = 90000,
            Description = "Универсальный зум для пейзажей и репортажей.",
            IsPopular = true
        },
        new Lens
        {
            Id = 3,
            Name = "Nikon AF-S 70-200mm f/2.8",
            Type = "sport",
            FocalLength = "70–200mm",
            MinFocal = 70,
            MaxFocal = 200,
            Aperture = "f/2.8",
            Compatibility = "Nikon F",
            Brand = "Nikon",
            Price = 130000,
            Description = "Телезум для спорта и съёмки с расстояния.",
            IsPopular = true
        },
        new Lens
        {
            Id = 4,
            Name = "Sony FE 90mm f/2.8 Macro",
            Type = "macro",
            FocalLength = "90mm",
            MinFocal = 90,
            MaxFocal = 90,
            Aperture = "f/2.8",
            Compatibility = "Sony FE",
            Brand = "Sony",
            Price = 110000,
            Description = "Макрообъектив для съёмки мелких деталей."
        },
        new Lens
        {
            Id = 5,
            Name = "Sigma 16mm f/1.4 DC DN",
            Type = "landscape",
            FocalLength = "16mm",
            MinFocal = 16,
            MaxFocal = 16,
            Aperture = "f/1.4",
            Compatibility = "Sony E / m4/3",
            Brand = "Sigma",
            Price = 45000,
            Description = "Широкоугольный объектив для пейзажей и интерьеров."
        }
    };

    public IEnumerable<Lens> GetAll() => _lenses;

    public Lens? GetById(int id) => _lenses.FirstOrDefault(l => l.Id == id);
}