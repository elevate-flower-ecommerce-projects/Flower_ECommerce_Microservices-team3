using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Data
{
    public static class FixedTestUsersSeeder
    {
        public static readonly Guid Customer1Id = Guid.Parse("01a08dfc-3620-71ae-8ac5-3e8c7b3f3c8b");
        public static readonly Guid Customer2Id = Guid.Parse("01a08dfc-3620-71ae-8ac5-3e8c7b3f3c8c");

        public static readonly Guid Driver1Id = Guid.Parse("01a03975-5bd9-7db4-8e83-599af35ee26c");
        public static readonly Guid Driver2Id = Guid.Parse("01a03975-5bd9-7db4-8e83-599af35ee26d");

        public static async Task SeedAsync(FlowersAuthDbContext context, IPasswordService passwordService)
        {
            var customerHash = passwordService.Hash("Customer@12345");
            var driverHash = passwordService.Hash("Driver@12345");

            var fixedUsers = new List<User>
            {
                // Customer 1 (Nourhan)
                new()
                {
                    Id = Customer1Id,
                    FirstName = "Nourhan",
                    LastName = "TrackingCustomer",
                    Email = "tracking.customer1@flowers.com",
                    HashPassword = customerHash,
                    Phone = "01099990001",
                    Gender = Gender.Female,
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                },
                // Customer 2 (Layla)
                new()
                {
                    Id = Customer2Id,
                    FirstName = "Layla",
                    LastName = "TrackingCustomer",
                    Email = "tracking.customer2@flowers.com",
                    HashPassword = customerHash,
                    Phone = "01099990002",
                    Gender = Gender.Female,
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                },
                // Driver 1 (Karim)
                new()
                {
                    Id = Driver1Id,
                    FirstName = "Karim",
                    LastName = "TrackingDriver",
                    Email = "tracking.driver1@flowers.com",
                    HashPassword = driverHash,
                    Phone = "01299990001",
                    PhotoUrl = "https://cdn.flowery-app.com/drivers/karim.jpg",
                    Gender = Gender.Male,
                    Role = UserRole.Driver,
                    CreatedAt = DateTime.UtcNow
                },
                // Driver 2 (Ahmed)
                new()
                {
                    Id = Driver2Id,
                    FirstName = "Ahmed",
                    LastName = "TrackingDriver",
                    Email = "tracking.driver2@flowers.com",
                    HashPassword = driverHash,
                    Phone = "01299990002",
                    PhotoUrl = "https://cdn.flowery-app.com/drivers/ahmed.jpg",
                    Gender = Gender.Male,
                    Role = UserRole.Driver,
                    CreatedAt = DateTime.UtcNow
                }
            };

            // 1. Seed or Update Users
            foreach (var user in fixedUsers)
            {
                var existingUser = await context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Id == user.Id || u.Email == user.Email);

                if (existingUser == null)
                {
                    await context.Users.AddAsync(user);
                }
                else
                {
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Email = user.Email;
                    existingUser.HashPassword = user.HashPassword;
                    existingUser.Phone = user.Phone;
                    existingUser.Role = user.Role;
                    if (!string.IsNullOrEmpty(user.PhotoUrl))
                    {
                        existingUser.PhotoUrl = user.PhotoUrl;
                    }
                }
            }
            await context.SaveChangesAsync();

            // 2. Ensure Customer records with matching Customer.Id == UserId
            var customerUserIds = new[] { Customer1Id, Customer2Id };
            foreach (var custId in customerUserIds)
            {
                var existingCustomer = await context.Customers
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(c => c.UserId == custId || c.Id == custId);

                if (existingCustomer == null)
                {
                    await context.Customers.AddAsync(new Customer
                    {
                        Id = custId,
                        UserId = custId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else if (existingCustomer.Id != custId)
                {
                    context.Customers.Remove(existingCustomer);
                    await context.SaveChangesAsync();

                    await context.Customers.AddAsync(new Customer
                    {
                        Id = custId,
                        UserId = custId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();

            // 3. Ensure Driver records with matching Driver.Id == UserId
            var driverUserIds = new[] { Driver1Id, Driver2Id };
            var adminUser = await context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Role == UserRole.Admin);

            foreach (var drvId in driverUserIds)
            {
                var existingDriver = await context.Drivers
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(d => d.UserId == drvId || d.Id == drvId);

                if (existingDriver == null)
                {
                    var application = new DriverApplication
                    {
                        Id = Guid.NewGuid(),
                        UserId = drvId,
                        VehicleType = VehicleType.Car,
                        VehicleNumber = drvId == Driver1Id ? "TRK-1020" : "TRK-3040",
                        VehicleLicenceImage = "https://cdn.flowery-app.com/licenses/default.png",
                        NationalIdNumber = drvId == Driver1Id ? "29901011234599" : "29901011234598",
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
                        Id = drvId,
                        UserId = drvId,
                        DriverApplicationId = application.Id,
                        VehicleType = VehicleType.Car,
                        VehicleNumber = application.VehicleNumber,
                        VehicleLicenceImage = application.VehicleLicenceImage,
                        NationalIdNumber = application.NationalIdNumber,
                        NationalIdImage = application.NationalIdImage,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else if (existingDriver.Id != drvId)
                {
                    var appId = existingDriver.DriverApplicationId;
                    context.Drivers.Remove(existingDriver);
                    await context.SaveChangesAsync();

                    await context.Drivers.AddAsync(new Driver
                    {
                        Id = drvId,
                        UserId = drvId,
                        DriverApplicationId = appId,
                        VehicleType = VehicleType.Car,
                        VehicleNumber = drvId == Driver1Id ? "TRK-1020" : "TRK-3040",
                        VehicleLicenceImage = "https://cdn.flowery-app.com/licenses/default.png",
                        NationalIdNumber = drvId == Driver1Id ? "29901011234599" : "29901011234598",
                        NationalIdImage = "https://cdn.flowery-app.com/national-ids/default.png",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
