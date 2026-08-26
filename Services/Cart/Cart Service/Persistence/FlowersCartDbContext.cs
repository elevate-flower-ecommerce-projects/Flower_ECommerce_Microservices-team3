using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Persistence
{
    public class FlowersCartDbContext : DbContext
    {
        public FlowersCartDbContext(DbContextOptions<FlowersCartDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowersCartDbContext).Assembly);
        }
    }
}
