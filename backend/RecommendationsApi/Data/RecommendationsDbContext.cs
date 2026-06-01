using Microsoft.EntityFrameworkCore;
using RecommendationsApi.Models;

namespace RecommendationsApi.Data;

public class RecommendationsDbContext : DbContext
{
    public DbSet<SelectionHistory> SelectionHistories { get; set; } = null!;

    public RecommendationsDbContext(DbContextOptions<RecommendationsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SelectionHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.ShootingType).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.ShootingType, e.CreatedAt });
        });
    }

    /// <summary>
    /// Профили типов съемки с рекомендуемыми характеристиками
    /// </summary>
    public static List<ShootingTypeProfile> GetShootingTypeProfiles()
    {
        return new List<ShootingTypeProfile>
        {
            new ShootingTypeProfile
            {
                Type = "portrait",
                Description = "Портретная съемка - фокус на лице, боке фон",
                PreferredBrands = ["Canon", "Nikon", "Sony"],
                PreferredApertureMin = 1, // f/1.8 и шире
                PreferredFocalMin = 50,
                PreferredFocalMax = 135
            },
            new ShootingTypeProfile
            {
                Type = "landscape",
                Description = "Пейзажная съемка - резкость по всей глубине",
                PreferredBrands = ["Canon", "Nikon"],
                PreferredApertureMin = 5, // f/5.6 и уже
                PreferredFocalMin = 14,
                PreferredFocalMax = 35
            },
            new ShootingTypeProfile
            {
                Type = "macro",
                Description = "Макрография - крупный план, высокая резкость",
                PreferredBrands = ["Canon", "Tamron"],
                PreferredApertureMin = 2, // f/2.8
                PreferredFocalMin = 90,
                PreferredFocalMax = 200
            },
            new ShootingTypeProfile
            {
                Type = "sports",
                Description = "Спортивная съемка - быстрая автофокусировка, широкое боке",
                PreferredBrands = ["Canon", "Nikon", "Sony"],
                PreferredApertureMin = 1, // f/2.8 и шире
                PreferredFocalMin = 70,
                PreferredFocalMax = 200
            }
        };
    }
}
