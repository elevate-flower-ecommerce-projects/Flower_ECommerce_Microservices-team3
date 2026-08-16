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
            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync())
            {
                var adminHash = passwordService.Hash("Admin@12345");
                var customerHash = passwordService.Hash("Customer@12345");
                var driverHash = passwordService.Hash("Driver@12345");

                var users = new List<User>
                {
                    // Admins
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "System",
                        LastName = "Admin",
                        Email = "admin@flowers.com",
                        HashPassword = adminHash,
                        Phone = "01000000001",
                        Gender = Gender.Male,
                        Role = UserRole.Admin
                    },
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "Super",
                        LastName = "Admin",
                        Email = "superadmin@flowers.com",
                        HashPassword = adminHash,
                        Phone = "01000000002",
                        Gender = Gender.Female,
                        Role = UserRole.Admin
                    },
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "Store",
                        LastName = "Manager",
                        Email = "manager@flowers.com",
                        HashPassword = adminHash,
                        Phone = "01000000003",
                        Gender = Gender.Male,
                        Role = UserRole.Admin
                    },

                    // Customers
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "Layla",
                        LastName = "Hassan",
                        Email = "customer1@flowers.com",
                        HashPassword = customerHash,
                        Phone = "01111111101",
                        Gender = Gender.Female,
                        Role = UserRole.Customer
                    },
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "Mohamed",
                        LastName = "Ali",
                        Email = "customer2@flowers.com",
                        HashPassword = customerHash,
                        Phone = "01111111102",
                        Gender = Gender.Male,
                        Role = UserRole.Customer
                    },
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@gmail.com",
                        HashPassword = customerHash,
                        Phone = "01111111103",
                        Gender = Gender.Male,
                        Role = UserRole.Customer
                    },
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "Sarah",
                        LastName = "Smith",
                        Email = "sarah.smith@gmail.com",
                        HashPassword = customerHash,
                        Phone = "01111111104",
                        Gender = Gender.Female,
                        Role = UserRole.Customer
                    },
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "Omar",
                        LastName = "Khaled",
                        Email = "omar.khaled@gmail.com",
                        HashPassword = customerHash,
                        Phone = "01111111105",
                        Gender = Gender.Male,
                        Role = UserRole.Customer
                    },

                    // Drivers
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "Ahmed",
                        LastName = "Driver",
                        Email = "driver1@flowers.com",
                        HashPassword = driverHash,
                        Phone = "01222222201",
                        Gender = Gender.Male,
                        Role = UserRole.Driver
                    },
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "Karim",
                        LastName = "Driver",
                        Email = "driver2@flowers.com",
                        HashPassword = driverHash,
                        Phone = "01222222202",
                        Gender = Gender.Male,
                        Role = UserRole.Driver
                    },
                    new User
                    {
                        Id = Guid.CreateVersion7(),
                        FirstName = "Tarek",
                        LastName = "Driver",
                        Email = "driver3@flowers.com",
                        HashPassword = driverHash,
                        Phone = "01222222203",
                        Gender = Gender.Male,
                        Role = UserRole.Driver
                    }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
        }
    }
}
