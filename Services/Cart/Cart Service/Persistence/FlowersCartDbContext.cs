using Cart_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Persistence
{
    public class FlowersCartDbContext : DbContext
    {
        public FlowersCartDbContext(DbContextOptions<FlowersCartDbContext> options) : base(options)
        {
        }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowersCartDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<CartItem>())
            {
                if (entry.State == EntityState.Modified && entry.Property(p => p.CartId).IsModified)
                {
                    entry.State = EntityState.Added;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
