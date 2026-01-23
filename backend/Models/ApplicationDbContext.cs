using Microsoft.EntityFrameworkCore;

namespace LensBackend.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Lens> Lenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Lens>().HasData(
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
                Name = "Canon EF 50mm f/1.4",
                Type = "portrait",
                FocalLength = "50mm",
                MinFocal = 50,
                MaxFocal = 50,
                Aperture = "f/1.4",
                Compatibility = "Canon EF",
                Brand = "Canon",
                Price = 25000,
                Description = "Стандартный объектив с большой диафрагмой для портретов."
            },
            new Lens
            {
                Id = 7,
                Name = "Nikon AF-S 50mm f/1.8G",
                Type = "portrait",
                FocalLength = "50mm",
                MinFocal = 50,
                MaxFocal = 50,
                Aperture = "f/1.8",
                Compatibility = "Nikon F",
                Brand = "Nikon",
                Price = 15000,
                Description = "Доступный портретный объектив с отличной оптикой."
            },
            new Lens
            {
                Id = 8,
                Name = "Sony FE 35mm f/1.8",
                Type = "portrait",
                FocalLength = "35mm",
                MinFocal = 35,
                MaxFocal = 35,
                Aperture = "f/1.8",
                Compatibility = "Sony FE",
                Brand = "Sony",
                Price = 40000,
                Description = "Широкоугольный портретный объектив для полнокадровых камер."
            },
            new Lens
            {
                Id = 9,
                Name = "Tamron 70-180mm f/2.8",
                Type = "sport",
                FocalLength = "70-180mm",
                MinFocal = 70,
                MaxFocal = 180,
                Aperture = "f/2.8",
                Compatibility = "Sony FE",
                Brand = "Tamron",
                Price = 95000,
                Description = "Легкий телезум для спорта и портретов."
            },
            new Lens
            {
                Id = 10,
                Name = "Sigma 24-70mm f/2.8 DG DN",
                Type = "landscape",
                FocalLength = "24-70mm",
                MinFocal = 24,
                MaxFocal = 70,
                Aperture = "f/2.8",
                Compatibility = "Sony FE / Leica L",
                Brand = "Sigma",
                Price = 85000,
                Description = "Высококачественный зум для пейзажной съемки."
            },
            new Lens
            {
                Id = 11,
                Name = "Canon EF-S 18-55mm f/3.5-5.6",
                Type = "landscape",
                FocalLength = "18-55mm",
                MinFocal = 18,
                MaxFocal = 55,
                Aperture = "f/3.5-5.6",
                Compatibility = "Canon EF-S",
                Brand = "Canon",
                Price = 12000,
                Description = "Бюджетный кит-объектив для начинающих."
            },
            new Lens
            {
                Id = 12,
                Name = "Nikon AF-P 18-55mm f/3.5-5.6G",
                Type = "landscape",
                FocalLength = "18-55mm",
                MinFocal = 18,
                MaxFocal = 55,
                Aperture = "f/3.5-5.6",
                Compatibility = "Nikon F",
                Brand = "Nikon",
                Price = 10000,
                Description = "Компактный зум для повседневной съемки."
            },
            new Lens
            {
                Id = 13,
                Name = "Sony E 16-55mm f/2.8 G",
                Type = "landscape",
                FocalLength = "16-55mm",
                MinFocal = 16,
                MaxFocal = 55,
                Aperture = "f/2.8",
                Compatibility = "Sony E",
                Brand = "Sony",
                Price = 120000,
                Description = "Профессиональный зум с постоянной диафрагмой."
            },
            new Lens
            {
                Id = 14,
                Name = "Sigma 100-400mm f/5-6.3 DG DN OS",
                Type = "sport",
                FocalLength = "100-400mm",
                MinFocal = 100,
                MaxFocal = 400,
                Aperture = "f/5-6.3",
                Compatibility = "Sony FE / Leica L",
                Brand = "Sigma",
                Price = 70000,
                Description = "Длиннофокусный зум для дикой природы."
            },
            new Lens
            {
                Id = 15,
                Name = "Tamron 17-70mm f/2.8-4",
                Type = "landscape",
                FocalLength = "17-70mm",
                MinFocal = 17,
                MaxFocal = 70,
                Aperture = "f/2.8-4",
                Compatibility = "Canon EF / Nikon F",
                Brand = "Tamron",
                Price = 50000,
                Description = "Универсальный зум с хорошей оптикой."
            },
            new Lens
            {
                Id = 16,
                Name = "Canon EF 100-400mm f/4.5-5.6L IS II",
                Type = "sport",
                FocalLength = "100-400mm",
                MinFocal = 100,
                MaxFocal = 400,
                Aperture = "f/4.5-5.6",
                Compatibility = "Canon EF",
                Brand = "Canon",
                Price = 180000,
                Description = "L-серия для профессиональной спортивной съемки."
            },
            new Lens
            {
                Id = 17,
                Name = "Nikon AF-S 80-400mm f/4.5-5.6G ED VR",
                Type = "sport",
                FocalLength = "80-400mm",
                MinFocal = 80,
                MaxFocal = 400,
                Aperture = "f/4.5-5.6",
                Compatibility = "Nikon F",
                Brand = "Nikon",
                Price = 160000,
                Description = "Мощный зум с оптической стабилизацией."
            },
            new Lens
            {
                Id = 18,
                Name = "Sony FE 100-400mm f/4.5-5.6 GM OSS",
                Type = "sport",
                FocalLength = "100-400mm",
                MinFocal = 100,
                MaxFocal = 400,
                Aperture = "f/4.5-5.6",
                Compatibility = "Sony FE",
                Brand = "Sony",
                Price = 220000,
                Description = "G Master зум для профессионалов."
            },
            new Lens
            {
                Id = 19,
                Name = "Sigma 150-600mm f/5-6.3 DG OS HSM",
                Type = "sport",
                FocalLength = "150-600mm",
                MinFocal = 150,
                MaxFocal = 600,
                Aperture = "f/5-6.3",
                Compatibility = "Canon EF / Nikon F / Sony FE",
                Brand = "Sigma",
                Price = 90000,
                Description = "Супер-телезум для птиц и спорта."
            },
            new Lens
            {
                Id = 20,
                Name = "Tamron 150-600mm f/5-6.3 Di VC USD G2",
                Type = "sport",
                FocalLength = "150-600mm",
                MinFocal = 150,
                MaxFocal = 600,
                Aperture = "f/5-6.3",
                Compatibility = "Canon EF / Nikon F",
                Brand = "Tamron",
                Price = 100000,
                Description = "Вторая версия с улучшенной оптикой."
            },
            new Lens
            {
                Id = 21,
                Name = "Canon EF 8-15mm f/4L Fisheye USM",
                Type = "landscape",
                FocalLength = "8-15mm",
                MinFocal = 8,
                MaxFocal = 15,
                Aperture = "f/4",
                Compatibility = "Canon EF",
                Brand = "Canon",
                Price = 120000,
                Description = "Фишай объектив для креативной съемки."
            },
            new Lens
            {
                Id = 22,
                Name = "Nikon AF-S 16-35mm f/4G ED VR",
                Type = "landscape",
                FocalLength = "16-35mm",
                MinFocal = 16,
                MaxFocal = 35,
                Aperture = "f/4",
                Compatibility = "Nikon F",
                Brand = "Nikon",
                Price = 110000,
                Description = "Широкоугольный зум с стабилизацией."
            },
            new Lens
            {
                Id = 23,
                Name = "Sony FE 12-24mm f/4 G",
                Type = "landscape",
                FocalLength = "12-24mm",
                MinFocal = 12,
                MaxFocal = 24,
                Aperture = "f/4",
                Compatibility = "Sony FE",
                Brand = "Sony",
                Price = 160000,
                Description = "Сверхширокоугольный зум для архитектуры."
            },
            new Lens
            {
                Id = 24,
                Name = "Sigma 14-24mm f/2.8 DG HSM",
                Type = "landscape",
                FocalLength = "14-24mm",
                MinFocal = 14,
                MaxFocal = 24,
                Aperture = "f/2.8",
                Compatibility = "Canon EF / Nikon F / Sony FE",
                Brand = "Sigma",
                Price = 130000,
                Description = "Высококачественный широкоугольник."
            },
            new Lens
            {
                Id = 25,
                Name = "Tamron 10-24mm f/3.5-4.5",
                Type = "landscape",
                FocalLength = "10-24mm",
                MinFocal = 10,
                MaxFocal = 24,
                Aperture = "f/3.5-4.5",
                Compatibility = "Canon EF / Nikon F",
                Brand = "Tamron",
                Price = 40000,
                Description = "Доступный сверхширокоугольный зум."
            },
            new Lens
            {
                Id = 26,
                Name = "Canon EF 100mm f/2.8L Macro IS USM",
                Type = "macro",
                FocalLength = "100mm",
                MinFocal = 100,
                MaxFocal = 100,
                Aperture = "f/2.8",
                Compatibility = "Canon EF",
                Brand = "Canon",
                Price = 80000,
                Description = "Макро-объектив с оптической стабилизацией."
            },
            new Lens
            {
                Id = 27,
                Name = "Nikon AF-S 105mm f/2.8G ED-IF VR",
                Type = "macro",
                FocalLength = "105mm",
                MinFocal = 105,
                MaxFocal = 105,
                Aperture = "f/2.8",
                Compatibility = "Nikon F",
                Brand = "Nikon",
                Price = 90000,
                Description = "Классический макро-объектив Nikon."
            },
            new Lens
            {
                Id = 28,
                Name = "Sony FE 90mm f/2.8 Macro G OSS",
                Type = "macro",
                FocalLength = "90mm",
                MinFocal = 90,
                MaxFocal = 90,
                Aperture = "f/2.8",
                Compatibility = "Sony FE",
                Brand = "Sony",
                Price = 100000,
                Description = "G-объектив для макросъемки."
            },
            new Lens
            {
                Id = 29,
                Name = "Sigma 70mm f/2.8 DG Macro",
                Type = "macro",
                FocalLength = "70mm",
                MinFocal = 70,
                MaxFocal = 70,
                Aperture = "f/2.8",
                Compatibility = "Canon EF / Nikon F / Sony FE",
                Brand = "Sigma",
                Price = 50000,
                Description = "Доступный макро-объектив с хорошей оптикой."
            },
            new Lens
            {
                Id = 30,
                Name = "Tamron 90mm f/2.8 Di VC USD Macro",
                Type = "macro",
                FocalLength = "90mm",
                MinFocal = 90,
                MaxFocal = 90,
                Aperture = "f/2.8",
                Compatibility = "Canon EF / Nikon F",
                Brand = "Tamron",
                Price = 55000,
                Description = "Макро с вибро-подавлением."
            },
            new Lens
            {
                Id = 31,
                Name = "Canon EF 400mm f/2.8L IS III USM",
                Type = "sport",
                FocalLength = "400mm",
                MinFocal = 400,
                MaxFocal = 400,
                Aperture = "f/2.8",
                Compatibility = "Canon EF",
                Brand = "Canon",
                Price = 1200000,
                Description = "Супер-телефото для профессионалов."
            },
            new Lens
            {
                Id = 32,
                Name = "Nikon AF-S 400mm f/2.8E FL ED VR",
                Type = "sport",
                FocalLength = "400mm",
                MinFocal = 400,
                MaxFocal = 400,
                Aperture = "f/2.8",
                Compatibility = "Nikon F",
                Brand = "Nikon",
                Price = 1300000,
                Description = "Флагманский телеобъектив Nikon."
            },
            new Lens
            {
                Id = 33,
                Name = "Sony FE 400mm f/2.8 GM OSS",
                Type = "sport",
                FocalLength = "400mm",
                MinFocal = 400,
                MaxFocal = 400,
                Aperture = "f/2.8",
                Compatibility = "Sony FE",
                Brand = "Sony",
                Price = 1400000,
                Description = "G Master 400мм для спорта и дикой природы."
            },
            new Lens
            {
                Id = 34,
                Name = "Sigma 50mm f/1.4 DG HSM",
                Type = "portrait",
                FocalLength = "50mm",
                MinFocal = 50,
                MaxFocal = 50,
                Aperture = "f/1.4",
                Compatibility = "Canon EF / Nikon F / Sony FE",
                Brand = "Sigma",
                Price = 60000,
                Description = "Арт-объектив с большой диафрагмой."
            },
            new Lens
            {
                Id = 35,
                Name = "Tamron 35mm f/1.8 VC",
                Type = "portrait",
                FocalLength = "35mm",
                MinFocal = 35,
                MaxFocal = 35,
                Aperture = "f/1.8",
                Compatibility = "Canon EF / Nikon F",
                Brand = "Tamron",
                Price = 30000,
                Description = "Широкоугольный объектив с стабилизацией."
            }
        );
    }
}