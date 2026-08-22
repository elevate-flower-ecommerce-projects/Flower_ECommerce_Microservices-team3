using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Identity.Infrastructure.Persistence.Data
{
    public class FlowersAuthDbContext : DbContext
    {
        public FlowersAuthDbContext(DbContextOptions<FlowersAuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<DriverApplication> DriverApplications => Set<DriverApplication>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<AdminLoginAudit> AdminLoginAudits => Set<AdminLoginAudit>();
        public DbSet<UserDevice> UserDevices => Set<UserDevice>();
        public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowersAuthDbContext).Assembly);
        }
    }
}
