using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Persistence.Seeding
{
    public static class CityAreaSeeder
    {
        private const string SeedActor = "system:seed";

        public static async Task SeedAsync(
            FlowersAddressStoreCoverageDbContext context,
            CancellationToken cancellationToken = default)
        {
            if (await context.Cities.IgnoreQueryFilters().AnyAsync(cancellationToken))
            {
                return;
            }

            // Nothing in this service stamps audit fields, so CreatedAt is set by hand.
            var seededAt = DateTime.UtcNow;

            var cities = new List<City>
        {
            new()
            {
                Id = new Guid("c1000000-0000-0000-0000-000000000001"),
                Name = "Cairo",
                CreatedAt = seededAt,
                CreatedBy = SeedActor,
                Areas = new List<Area>
                {
                    NewArea("a1000000-0000-0000-0000-000000000001", "Maadi", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000002", "Nasr City", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000003", "Heliopolis", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000004", "Zamalek", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000005", "Downtown", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000006", "New Cairo", seededAt),
                }
            },
            new()
            {
                Id = new Guid("c1000000-0000-0000-0000-000000000002"),
                Name = "Giza",
                CreatedAt = seededAt,
                CreatedBy = SeedActor,
                Areas = new List<Area>
                {
                    NewArea("a1000000-0000-0000-0000-000000000101", "Dokki", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000102", "Mohandessin", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000103", "Haram", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000104", "Sheikh Zayed", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000105", "6th of October", seededAt),
                }
            },
            new()
            {
                Id = new Guid("c1000000-0000-0000-0000-000000000003"),
                Name = "Alexandria",
                CreatedAt = seededAt,
                CreatedBy = SeedActor,
                Areas = new List<Area>
                {
                    NewArea("a1000000-0000-0000-0000-000000000201", "Smouha", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000202", "Sidi Gaber", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000203", "Gleem", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000204", "Miami", seededAt),
                    NewArea("a1000000-0000-0000-0000-000000000205", "Montazah", seededAt),
                }
            },
        };

            context.Cities.AddRange(cities);
            await context.SaveChangesAsync(cancellationToken);
        }

        // CityId is left unset on purpose — EF assigns it from the owning City's Areas collection.
        private static Area NewArea(string id, string name, DateTime seededAt) => new()
        {
            Id = new Guid(id),
            Name = name,
            CreatedAt = seededAt,
            CreatedBy = SeedActor
        };
    }
}
