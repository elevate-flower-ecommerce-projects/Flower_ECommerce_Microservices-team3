using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Persistence
{
    public class FlowersAddressStoreCoverageDbContext : DbContext
    {
        public FlowersAddressStoreCoverageDbContext(DbContextOptions<FlowersAddressStoreCoverageDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowersAddressStoreCoverageDbContext).Assembly);
        }
    }
}

