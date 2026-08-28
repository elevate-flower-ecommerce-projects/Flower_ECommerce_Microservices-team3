using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Persistence.Seeding
{
    public static class CoverageAreaSeeder
    {
        public static async Task SeedAsync(FlowersAddressStoreCoverageDbContext db)
        {
            if (await db.CoverageAreas.IgnoreQueryFilters().AnyAsync()) return;

            var stores = await db.Stores.IgnoreQueryFilters().ToListAsync();
            if (!stores.Any()) return;

            var nasrCityStore = stores.FirstOrDefault(s => s.Name.Contains("Nasr City"));
            var maadiStore = stores.FirstOrDefault(s => s.Name.Contains("Maadi"));
            var heliopolisStore = stores.FirstOrDefault(s => s.Name.Contains("Heliopolis"));

            var coverageAreas = new List<CoverageArea>();

            if (nasrCityStore != null)
            {
                coverageAreas.Add(new CoverageArea
                {
                    StoreId = nasrCityStore.Id,
                    BoundaryType = CoverageBoundaryType.Radius,
                    RadiusMeters = 10000,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (maadiStore != null)
            {
                coverageAreas.Add(new CoverageArea
                {
                    StoreId = maadiStore.Id,
                    BoundaryType = CoverageBoundaryType.Polygon,
                    Polygon = new List<GeoPoint>
                    {
                        new(29.9750, 31.2400),
                        new(29.9800, 31.2800),
                        new(29.9450, 31.2850),
                        new(29.9400, 31.2450),
                        new(29.9750, 31.2400)
                    },
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (heliopolisStore != null)
            {
                coverageAreas.Add(new CoverageArea
                {
                    StoreId = heliopolisStore.Id,
                    BoundaryType = CoverageBoundaryType.CityAreaList,
                    Cities = new List<string> { "Cairo" },
                    Areas = new List<string> { "Heliopolis", "Al-Nozha", "Sheraton", "Nasr City" },
                    CreatedAt = DateTime.UtcNow
                });
            }

            foreach (var store in stores)
            {
                if (coverageAreas.All(c => c.StoreId != store.Id))
                {
                    coverageAreas.Add(new CoverageArea
                    {
                        StoreId = store.Id,
                        BoundaryType = CoverageBoundaryType.Radius,
                        RadiusMeters = (store.CoverageRadiusKm > 0 ? store.CoverageRadiusKm : 5) * 1000,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (coverageAreas.Any())
            {
                db.CoverageAreas.AddRange(coverageAreas);
                await db.SaveChangesAsync();
            }
        }
    }
}
