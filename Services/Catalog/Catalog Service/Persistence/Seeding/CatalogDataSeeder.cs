using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Persistence.Seeding;

public static class CatalogDataSeeder
{
    public static async Task SeedAsync(FlowersCatalogDbContext db)
    {
        // 1. Seed Categories if empty
        if (!await db.Categories.IgnoreQueryFilters().AnyAsync())
        {
            var categories = new List<Category>
            {
                new()
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Roses",
                    NameAr = "ورود",
                    Icon = "roses.png",
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Tulips",
                    NameAr = "توليب",
                    Icon = "tulips.png",
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Bouquets",
                    NameAr = "باقات زهور",
                    Icon = "bouquets.png",
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await db.Categories.AddRangeAsync(categories);
            await db.SaveChangesAsync();
        }

        // 2. Seed Occasions if empty
        if (!await db.Occasions.IgnoreQueryFilters().AnyAsync())
        {
            var occasions = new List<Occasion>
            {
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Birthday",
                    NameAr = "عيد ميلاد",
                    ImageUrl = "https://images.unsplash.com/photo-1558636508-e0db3814bd1d?auto=format&fit=crop&w=800&q=80",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Anniversary",
                    NameAr = "ذكرى سنوية",
                    ImageUrl = "https://images.unsplash.com/photo-1515934751635-c81c6bc9a2d8?auto=format&fit=crop&w=800&q=80",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Name = "Valentine's Day",
                    NameAr = "عيد الحب",
                    ImageUrl = "https://images.unsplash.com/photo-1518199266791-5375a83190b7?auto=format&fit=crop&w=800&q=80",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await db.Occasions.AddRangeAsync(occasions);
            await db.SaveChangesAsync();
        }

        // 3. Seed Products if empty
        if (!await db.Products.IgnoreQueryFilters().AnyAsync())
        {
            var bouquetsCategory = await db.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Name == "Bouquets")
                                   ?? await db.Categories.IgnoreQueryFilters().FirstAsync();

            var birthdayOccasion = await db.Occasions.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Name.Contains("Birthday"))
                                   ?? await db.Occasions.IgnoreQueryFilters().FirstAsync();

            var anniversaryOccasion = await db.Occasions.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Name.Contains("Anniversary"))
                                     ?? birthdayOccasion;

            var valentinesOccasion = await db.Occasions.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Name.Contains("Valentine"))
                                    ?? birthdayOccasion;

            var product1 = new Product
            {
                Name = "Red roses",
                NameAr = "ورود حمراء",
                ImageUrl = "https://cdn.flowery-app.com/products/101.jpg",
                Currency = "EGP",
                Price = 600,
                OriginalPrice = 800,
                DiscountPercentage = 25,
                Status = ProductStatus.InStock,
                Description = "Beautiful bouquet of fresh red roses for your special occasions.",
                DescriptionAr = "باقة جميلة من الورود الحمراء الطازجة لمناسباتك الخاصة.",
                IsBestSeller = true,
                BestSellerOrder = 1,
                CategoryId = bouquetsCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Images =
                [
                    new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/101_1.jpg", SortOrder = 0 },
                    new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/101_2.jpg", SortOrder = 1 }
                ],
                Includes =
                [
                    new ProductInclude { Name = "Red roses", NameAr = "ورود حمراء" },
                    new ProductInclude { Name = "Black wrap", NameAr = "تغليف أسود" }
                ]
            };

            var product2 = new Product
            {
                Name = "Sunny",
                NameAr = "صاني",
                ImageUrl = "https://cdn.flowery-app.com/products/102.jpg",
                Currency = "EGP",
                Price = 600,
                OriginalPrice = null,
                DiscountPercentage = null,
                Status = ProductStatus.InStock,
                Description = "Bright yellow sunflowers that bring warmth and joy.",
                DescriptionAr = "دوّار شمس أصفر مشرق يبعث الدفء والبهجة.",
                IsBestSeller = true,
                BestSellerOrder = 2,
                CategoryId = bouquetsCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Images =
                [
                    new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/102_1.jpg", SortOrder = 0 }
                ],
                Includes =
                [
                    new ProductInclude { Name = "Sunflowers", NameAr = "عباد الشمس" },
                    new ProductInclude { Name = "Yellow ribbon", NameAr = "شريط أصفر" }
                ]
            };

            var product3 = new Product
            {
                Name = "15 Pink Rose Bouquet",
                NameAr = "باقة ١٥ وردة وردية",
                ImageUrl = "https://cdn.flowery-app.com/products/103.jpg",
                Currency = "EGP",
                Price = 1500,
                OriginalPrice = null,
                DiscountPercentage = null,
                Status = ProductStatus.InStock,
                Description = "Fresh fragrant roses arranged into a lovely bouquet.",
                DescriptionAr = "وصف منتج تجريبي للباقة الوردية الأنيقة.",
                IsBestSeller = true,
                BestSellerOrder = 3,
                CategoryId = bouquetsCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Images =
                [
                    new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/103_1.jpg", SortOrder = 0 },
                    new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/103_2.jpg", SortOrder = 1 }
                ],
                Includes =
                [
                    new ProductInclude { Name = "Pink roses: 15", NameAr = "ورد وردي: ١٥" },
                    new ProductInclude { Name = "White wrap", NameAr = "تغليف أبيض" }
                ]
            };

            await db.Products.AddRangeAsync(product1, product2, product3);
            await db.SaveChangesAsync();

            // Link Products & Occasions
            await db.ProductOccasions.AddRangeAsync(
                new ProductOccasion { ProductId = product1.Id, OccasionId = valentinesOccasion.Id },
                new ProductOccasion { ProductId = product1.Id, OccasionId = anniversaryOccasion.Id },
                new ProductOccasion { ProductId = product2.Id, OccasionId = birthdayOccasion.Id },
                new ProductOccasion { ProductId = product3.Id, OccasionId = valentinesOccasion.Id },
                new ProductOccasion { ProductId = product3.Id, OccasionId = birthdayOccasion.Id }
            );
            await db.SaveChangesAsync();
        }

        // 4. Seed Home Sections if empty
        if (!await db.HomeSections.IgnoreQueryFilters().AnyAsync())
        {
            var valentinesOccasion = await db.Occasions.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Name.Contains("Valentine"));

            await db.HomeSections.AddRangeAsync(
                new HomeSection
                {
                    Type = HomeSectionType.Categories,
                    Title = "Categories",
                    TitleAr = "الفئات",
                    Index = 0,
                    CreatedAt = DateTime.UtcNow
                },
                new HomeSection
                {
                    Type = HomeSectionType.BestSeller,
                    Title = "Best seller",
                    TitleAr = "الأكثر مبيعاً",
                    Index = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new HomeSection
                {
                    Type = HomeSectionType.Occasions,
                    Title = "Occasion",
                    TitleAr = "المناسبات",
                    Index = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new HomeSection
                {
                    Type = HomeSectionType.ProductsCarousel,
                    Title = "Valentine's picks",
                    TitleAr = "اختيارات عيد الحب",
                    Index = 3,
                    OccasionId = valentinesOccasion?.Id,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await db.SaveChangesAsync();
        }
    }
}