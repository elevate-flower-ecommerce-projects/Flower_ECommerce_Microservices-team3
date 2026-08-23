using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Persistence
{
    public class FlowersAddressStoreCoverageDbContext : DbContext
    {
        public FlowersAddressStoreCoverageDbContext(DbContextOptions<FlowersAddressStoreCoverageDbContext> options) : base(options)
        {
        }

        public DbSet<City> Cities => Set<City>();
        public DbSet<Area> Areas => Set<Area>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Store> Stores => Set<Store>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowersAddressStoreCoverageDbContext).Assembly);
        }
    }
}

