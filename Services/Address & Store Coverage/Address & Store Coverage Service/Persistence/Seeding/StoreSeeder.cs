using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;
namespace Address___Store_Coverage_Service.Persistence.Seeding
{
    public static class StoreSeeder
    {
        public static async Task SeedAsync(FlowersAddressStoreCoverageDbContext db)
        {
            if (await db.Stores.IgnoreQueryFilters().AnyAsync()) return;
            db.Stores.AddRange(
                new Store
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Nasr City Branch",
                    Latitude = 30.0511,
                    Longitude = 31.3656,
                    CoverageRadiusKm = 10,
                    CreatedAt = DateTime.UtcNow
                },
                new Store
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Maadi Branch",
                    Latitude = 29.9602,
                    Longitude = 31.2569,
                    CoverageRadiusKm = 8,
                    CreatedAt = DateTime.UtcNow
                },
                new Store
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Heliopolis Branch",
                    Latitude = 30.0866,
                    Longitude = 31.3225,
                    CoverageRadiusKm = 7,
                    CreatedAt = DateTime.UtcNow
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
