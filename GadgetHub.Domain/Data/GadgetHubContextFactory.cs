using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GadgetHub.Domain.Data
{
    public class GadgetHubContextFactory : IDesignTimeDbContextFactory<GadgetHubContext>
    {
        public GadgetHubContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GadgetHubContext>();
            // Use the same connection string as in your startup project
            optionsBuilder.UseSqlite("Data Source=GadgetHub.db");

            return new GadgetHubContext(optionsBuilder.Options);
        }
    }
}