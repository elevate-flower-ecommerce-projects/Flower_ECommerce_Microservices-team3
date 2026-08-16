using Catalog_Service.Entities;
using Catalog_Service.Persistence;

namespace Catalog_Service.Persistence.Seeding;

public static class CatalogDataSeeder
{
    public static async Task SeedAsync(
        FlowersCatalogDbContext context)
    {
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.Parse(
                        "11111111-1111-1111-1111-111111111111"),
                    Name = "Roses",
                    Icon = "roses.png",
                    IsActive = true,
                    DisplayOrder = 1
                },

                new Category
                {
                    Id = Guid.Parse(
                        "22222222-2222-2222-2222-222222222222"),
                    Name = "Tulips",
                    Icon = "tulips.png",
                    IsActive = true,
                    DisplayOrder = 2
                },

                new Category
                {
                    Id = Guid.Parse(
                        "33333333-3333-3333-3333-333333333333"),
                    Name = "Bouquets",
                    Icon = "bouquets.png",
                    IsActive = true,
                    DisplayOrder = 3
                }
            };

            await context.Categories.AddRangeAsync(categories);

            await context.SaveChangesAsync();
        }
    }
}