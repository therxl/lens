using Microsoft.EntityFrameworkCore;
using LensApi.Models;

namespace LensApi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Lens> Lenses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Lens entity
        modelBuilder.Entity<Lens>(entity =>
        {
            entity.ToTable("lenses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.Type).HasColumnName("type").IsRequired();
            entity.Property(e => e.FocalLength).HasColumnName("focal_length").IsRequired();
            entity.Property(e => e.MinFocal).HasColumnName("min_focal").IsRequired();
            entity.Property(e => e.MaxFocal).HasColumnName("max_focal").IsRequired();
            entity.Property(e => e.Aperture).HasColumnName("aperture").IsRequired();
            entity.Property(e => e.Compatibility).HasColumnName("compatibility").IsRequired();
            entity.Property(e => e.Brand).HasColumnName("brand").IsRequired();
            entity.Property(e => e.Price).HasColumnName("price").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsPopular).HasColumnName("is_popular").HasDefaultValue(false);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.Username).HasColumnName("username").IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Mode).HasColumnName("mode").HasDefaultValue("user");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure Favorite entity
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("favorites");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.LensId).HasColumnName("lens_id").IsRequired();
            entity.Property(e => e.AddedAt).HasColumnName("added_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Lens>()
                .WithMany()
                .HasForeignKey(e => e.LensId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}