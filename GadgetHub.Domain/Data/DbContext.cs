using GadgetHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.Domain.Data;

public class GadgetHubContext : DbContext
{
    public GadgetHubContext(DbContextOptions<GadgetHubContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Product>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }
}