using Microsoft.EntityFrameworkCore;
using Payment_Service.Entities;
using Payment_Service.Persistence.Configurations;

namespace Payment_Service.Persistence
{
    public class FlowersPaymentDbContext : DbContext
    {
        public FlowersPaymentDbContext(DbContextOptions<FlowersPaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<PaymentWebhookEvent> PaymentWebhookEvents =>
            Set<PaymentWebhookEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                    typeof(PaymentConfiguration).Assembly);
        }
    }
}