using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Persistence;

public class FlowersCatalogDbContext : DbContext
{
    public FlowersCatalogDbContext(DbContextOptions<FlowersCatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Occasion> Occasions => Set<Occasion>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductInclude> ProductIncludes => Set<ProductInclude>();
    public DbSet<ProductOccasion> ProductOccasions => Set<ProductOccasion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowersCatalogDbContext).Assembly);
    }
}
