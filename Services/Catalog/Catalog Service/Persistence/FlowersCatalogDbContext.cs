using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Persistence
{
    public class FlowersCatalogDbContext : DbContext
    {
        public FlowersCatalogDbContext(DbContextOptions<FlowersCatalogDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowersCatalogDbContext).Assembly);
        }
    }
}
