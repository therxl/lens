using LensApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LensApi.Repositories;

public class LensRepository : ILensRepository
{
    private readonly ApplicationDbContext _context;

    public LensRepository(ApplicationDbContext context)
    {
        _context = context;
        SeedData();
    }

    private void SeedData()
    {
        var seedLenses = new List<Lens>
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
            },
            new Lens
            {
                Id = 6,
                Name = "Canon RF 50mm f/1.2L",
                Type = "portrait",
                FocalLength = "50mm",
                MinFocal = 50,
                MaxFocal = 50,
                Aperture = "f/1.2",
                Compatibility = "Canon RF",
                Brand = "Canon",
                Price = 175000,
                Description = "Светосильный фикс для выразительных портретов.",
                IsPopular = true
            },
            new Lens
            {
                Id = 7,
                Name = "Nikon Z 24-120mm f/4 S",
                Type = "landscape",
                FocalLength = "24–120mm",
                MinFocal = 24,
                MaxFocal = 120,
                Aperture = "f/4",
                Compatibility = "Nikon Z",
                Brand = "Nikon",
                Price = 115000,
                Description = "Универсальный тревел-зум на каждый день.",
                IsPopular = true
            },
            new Lens
            {
                Id = 8,
                Name = "Sony FE 24mm f/1.4 GM",
                Type = "landscape",
                FocalLength = "24mm",
                MinFocal = 24,
                MaxFocal = 24,
                Aperture = "f/1.4",
                Compatibility = "Sony FE",
                Brand = "Sony",
                Price = 149000,
                Description = "Широкий угол и высокая резкость по всему кадру.",
                IsPopular = true
            },
            new Lens
            {
                Id = 9,
                Name = "Tamron 35-150mm f/2-2.8",
                Type = "portrait",
                FocalLength = "35–150mm",
                MinFocal = 35,
                MaxFocal = 150,
                Aperture = "f/2-2.8",
                Compatibility = "Sony FE / Nikon Z",
                Brand = "Tamron",
                Price = 165000,
                Description = "Гибкий диапазон фокусных для портретов и событий."
            },
            new Lens
            {
                Id = 10,
                Name = "Fujifilm XF 56mm f/1.2 R",
                Type = "portrait",
                FocalLength = "56mm",
                MinFocal = 56,
                MaxFocal = 56,
                Aperture = "f/1.2",
                Compatibility = "Fujifilm X",
                Brand = "Fujifilm",
                Price = 95000,
                Description = "Портретный фикс с мягким боке и точным фокусом.",
                IsPopular = true
            },
            new Lens
            {
                Id = 11,
                Name = "Olympus M.Zuiko 60mm f/2.8 Macro",
                Type = "macro",
                FocalLength = "60mm",
                MinFocal = 60,
                MaxFocal = 60,
                Aperture = "f/2.8",
                Compatibility = "Micro 4/3",
                Brand = "Olympus",
                Price = 48000,
                Description = "Компактный макрообъектив для предметной съёмки."
            },
            new Lens
            {
                Id = 12,
                Name = "Laowa 100mm f/2.8 2X Ultra Macro",
                Type = "macro",
                FocalLength = "100mm",
                MinFocal = 100,
                MaxFocal = 100,
                Aperture = "f/2.8",
                Compatibility = "Canon EF / Nikon F / Sony FE",
                Brand = "Laowa",
                Price = 72000,
                Description = "Макро 2X для экстремальной детализации."
            },
            new Lens
            {
                Id = 13,
                Name = "Canon RF 70-200mm f/2.8L",
                Type = "sport",
                FocalLength = "70–200mm",
                MinFocal = 70,
                MaxFocal = 200,
                Aperture = "f/2.8",
                Compatibility = "Canon RF",
                Brand = "Canon",
                Price = 210000,
                Description = "Профессиональный теле-зум для спорта и событий.",
                IsPopular = true
            },
            new Lens
            {
                Id = 14,
                Name = "Sony FE 200-600mm f/5.6-6.3 G",
                Type = "sport",
                FocalLength = "200–600mm",
                MinFocal = 200,
                MaxFocal = 600,
                Aperture = "f/5.6-6.3",
                Compatibility = "Sony FE",
                Brand = "Sony",
                Price = 175000,
                Description = "Супертелеобъектив для дикой природы и спорта."
            },
            new Lens
            {
                Id = 15,
                Name = "Tokina 11-20mm f/2.8",
                Type = "landscape",
                FocalLength = "11–20mm",
                MinFocal = 11,
                MaxFocal = 20,
                Aperture = "f/2.8",
                Compatibility = "Canon EF / Nikon F",
                Brand = "Tokina",
                Price = 68000,
                Description = "Сверхширокий зум для архитектуры и пейзажей."
            },
            new Lens
            {
                Id = 16,
                Name = "Samyang 135mm f/2",
                Type = "portrait",
                FocalLength = "135mm",
                MinFocal = 135,
                MaxFocal = 135,
                Aperture = "f/2",
                Compatibility = "Canon EF / Nikon F / Sony FE",
                Brand = "Samyang",
                Price = 53000,
                Description = "Длинный фикс для сжатой перспективы и боке."
            },
            new Lens
            {
                Id = 17,
                Name = "Panasonic Leica 12-60mm f/2.8-4",
                Type = "landscape",
                FocalLength = "12–60mm",
                MinFocal = 12,
                MaxFocal = 60,
                Aperture = "f/2.8-4",
                Compatibility = "Micro 4/3",
                Brand = "Panasonic",
                Price = 82000,
                Description = "Универсальный тревел-зум с хорошей резкостью."
            },
            new Lens
            {
                Id = 18,
                Name = "Fujifilm XF 16-55mm f/2.8 R LM WR",
                Type = "landscape",
                FocalLength = "16–55mm",
                MinFocal = 16,
                MaxFocal = 55,
                Aperture = "f/2.8",
                Compatibility = "Fujifilm X",
                Brand = "Fujifilm",
                Price = 132000,
                Description = "Рабочая лошадка для пейзажей, людей и репортажей.",
                IsPopular = true
            },
            new Lens
            {
                Id = 19,
                Name = "Pentax HD DA 40mm f/2.8 Limited",
                Type = "portrait",
                FocalLength = "40mm",
                MinFocal = 40,
                MaxFocal = 40,
                Aperture = "f/2.8",
                Compatibility = "Pentax K",
                Brand = "Pentax",
                Price = 32000,
                Description = "Компактный фикс для повседневной съёмки."
            },
            new Lens
            {
                Id = 20,
                Name = "Zeiss Batis 135mm f/2.8",
                Type = "portrait",
                FocalLength = "135mm",
                MinFocal = 135,
                MaxFocal = 135,
                Aperture = "f/2.8",
                Compatibility = "Sony FE",
                Brand = "Zeiss",
                Price = 138000,
                Description = "Премиальный портретный телеобъектив."
            }
        };

        if (!_context.Lenses.Any())
        {
            _context.Lenses.AddRange(seedLenses);
            _context.SaveChanges();
            return;
        }

        var existingIds = _context.Lenses.Select(l => l.Id).ToHashSet();
        var missingLenses = seedLenses.Where(l => !existingIds.Contains(l.Id)).ToList();

        if (missingLenses.Count > 0)
        {
            _context.Lenses.AddRange(missingLenses);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Lens> GetAllLenses() => _context.Lenses.ToList();

    public Lens? GetLensById(int id) => _context.Lenses.FirstOrDefault(l => l.Id == id);

    public IEnumerable<Lens> GetPopularLenses()
    {
        return _context.Lenses.FromSqlRaw("SELECT * FROM lenses WHERE is_popular = true").ToList();
    }

    public IEnumerable<Lens> GetLensesByBrand(string brand)
    {
        return _context.Lenses.FromSqlRaw("SELECT * FROM lenses WHERE brand = {0}", brand).ToList();
    }

    public IEnumerable<Lens> GetLensesByType(string type)
    {
        return _context.Lenses.FromSqlRaw("SELECT * FROM lenses WHERE type = {0}", type).ToList();
    }
}