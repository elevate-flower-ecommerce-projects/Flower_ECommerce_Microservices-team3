using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;

namespace Order___Fulfillment_Service.Persistence;

public class FlowersOrderDbContext : DbContext
{
    public FlowersOrderDbContext(DbContextOptions<FlowersOrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowersOrderDbContext).Assembly);
    }
}
