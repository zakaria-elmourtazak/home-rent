using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyMvcAuthProject.Models;

namespace MyMvcAuthProject.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Models.Property> Properties { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One Property -> Many Amenities
        modelBuilder.Entity<Models.Property>()
            .HasMany(p => p.Amenities)
            .WithOne(a => a.Property)
            .HasForeignKey(a => a.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // One Property -> Many Images
        modelBuilder.Entity<Models.Property>()
            .HasMany(p => p.PropertyImages)
            .WithOne(i => i.Property)
            .HasForeignKey(i => i.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
