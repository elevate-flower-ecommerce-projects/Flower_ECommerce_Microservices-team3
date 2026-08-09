using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Persistence.Data
{
    public class FlowersAuthSeeder
    {
        public static async Task SeedAsync(FlowersAuthDbContext context, IPasswordService passwordService)
        {
            if (!context.Users.Any(u => u.Role == UserRole.Admin))
            {
                var adminUser = new User
                {
                    Id = Guid.CreateVersion7(),
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@flowers.com",
                    HashPassword = passwordService.Hash("Admin@12345"),
                    Phone = "01000000000",
                    Gender = Gender.Male,
                    Role = UserRole.Admin
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
