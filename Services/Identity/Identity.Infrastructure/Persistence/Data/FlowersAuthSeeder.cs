using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Data
{
    public class FlowersAuthSeeder
    {
        public static async Task SeedAsync(FlowersAuthDbContext context, IPasswordService passwordService)
        {
            var adminHash = passwordService.Hash("Admin@12345");
            var customerHash = passwordService.Hash("Customer@12345");
            var driverHash = passwordService.Hash("Driver@12345");

            var seedUsers = new List<User>
            {
                // Admins
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@flowers.com",
                    HashPassword = adminHash,
                    Phone = "01000000001",
                    Gender = Gender.Male,
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = "superadmin@flowers.com",
                    HashPassword = adminHash,
                    Phone = "01000000002",
                    Gender = Gender.Female,
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "Store",
                    LastName = "Manager",
                    Email = "manager@flowers.com",
                    HashPassword = adminHash,
                    Phone = "01000000003",
                    Gender = Gender.Male,
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                },

                // Customers
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "Layla",
                    LastName = "Hassan",
                    Email = "customer1@flowers.com",
                    HashPassword = customerHash,
                    Phone = "01111111101",
                    Gender = Gender.Female,
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "Mohamed",
                    LastName = "Ali",
                    Email = "customer2@flowers.com",
                    HashPassword = customerHash,
                    Phone = "01111111102",
                    Gender = Gender.Male,
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@gmail.com",
                    HashPassword = customerHash,
                    Phone = "01111111103",
                    Gender = Gender.Male,
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "Sarah",
                    LastName = "Smith",
                    Email = "sarah.smith@gmail.com",
                    HashPassword = customerHash,
                    Phone = "01111111104",
                    Gender = Gender.Female,
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "Omar",
                    LastName = "Khaled",
                    Email = "omar.khaled@gmail.com",
                    HashPassword = customerHash,
                    Phone = "01111111105",
                    Gender = Gender.Male,
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                },

                // Drivers
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "Ahmed",
                    LastName = "Driver",
                    Email = "driver1@flowers.com",
                    HashPassword = driverHash,
                    Phone = "01222222201",
                    Gender = Gender.Male,
                    Role = UserRole.Driver,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "Karim",
                    LastName = "Driver",
                    Email = "driver2@flowers.com",
                    HashPassword = driverHash,
                    Phone = "01222222202",
                    Gender = Gender.Male,
                    Role = UserRole.Driver,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "Tarek",
                    LastName = "Driver",
                    Email = "driver3@flowers.com",
                    HashPassword = driverHash,
                    Phone = "01222222203",
                    Gender = Gender.Male,
                    Role = UserRole.Driver,
                    CreatedAt = DateTime.UtcNow
                }
            };

            // 1. Seed any missing users by Email
            foreach (var user in seedUsers)
            {
                var existingUser = await context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser == null)
                {
                    await context.Users.AddAsync(user);
                }
            }
            await context.SaveChangesAsync();

            // 2. Ensure all Customers have a corresponding Customer record
            var customerUsers = await context.Users
                .IgnoreQueryFilters()
                .Where(u => u.Role == UserRole.Customer)
                .ToListAsync();

            foreach (var customerUser in customerUsers)
            {
                var hasCustomerRecord = await context.Customers
                    .IgnoreQueryFilters()
                    .AnyAsync(c => c.UserId == customerUser.Id);

                if (!hasCustomerRecord)
                {
                    await context.Customers.AddAsync(new Customer
                    {
                        Id = Guid.CreateVersion7(),
                        UserId = customerUser.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();

            // 3. Ensure all Drivers have Driver + DriverApplication records
            var driverUsers = await context.Users
                .IgnoreQueryFilters()
                .Where(u => u.Role == UserRole.Driver)
                .ToListAsync();

            var adminUser = await context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Role == UserRole.Admin);

            foreach (var driverUser in driverUsers)
            {
                var hasDriverRecord = await context.Drivers
                    .IgnoreQueryFilters()
                    .AnyAsync(d => d.UserId == driverUser.Id);

                if (!hasDriverRecord)
                {
                    var application = new DriverApplication
                    {
                        Id = Guid.CreateVersion7(),
                        UserId = driverUser.Id,
                        VehicleType = VehicleType.Car,
                        VehicleNumber = "ABC-1234",
                        VehicleLicenceImage = "https://cdn.flowery-app.com/licenses/default.png",
                        NationalIdNumber = "29901011234567",
                        NationalIdImage = "https://cdn.flowery-app.com/national-ids/default.png",
                        CreatedAt = DateTime.UtcNow
                    };

                    if (adminUser != null)
                    {
                        application.Approve(adminUser.Id);
                    }

                    await context.DriverApplications.AddAsync(application);

                    await context.Drivers.AddAsync(new Driver
                    {
                        Id = Guid.CreateVersion7(),
                        UserId = driverUser.Id,
                        DriverApplicationId = application.Id,
                        VehicleType = VehicleType.Car,
                        VehicleNumber = "ABC-1234",
                        VehicleLicenceImage = "https://cdn.flowery-app.com/licenses/default.png",
                        NationalIdNumber = "29901011234567",
                        NationalIdImage = "https://cdn.flowery-app.com/national-ids/default.png",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
