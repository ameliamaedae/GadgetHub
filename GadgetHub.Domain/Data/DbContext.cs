using Microsoft.EntityFrameworkCore;
using GadgetHub.Domain.Entities;

namespace GadgetHub.Domain.Data
{
    public class GadgetHubContext : DbContext
    {
        public GadgetHubContext(DbContextOptions<GadgetHubContext> options)
            : base(options)
        {
        }
        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        // Optionally, override OnModelCreating to configure relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // For example, configure precision for Price:
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}