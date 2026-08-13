using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Persistence.Seeding;

public static class CatalogDataSeeder
{
    public static async Task SeedAsync(FlowersCatalogDbContext db)
    {
        if (await db.Products.AnyAsync())
            return;

        // 1. Seed Categories
        var bouquetsCategory = new Category
        {
            Name = "Bouquets",
            NameAr = "باقات زهور",
            CreatedAt = DateTime.UtcNow
        };

        var arrangementsCategory = new Category
        {
            Name = "Arrangements",
            NameAr = "تنسیقات زهور",
            CreatedAt = DateTime.UtcNow
        };

        var singleStemsCategory = new Category
        {
            Name = "Single Stems",
            NameAr = "زهور فردية",
            CreatedAt = DateTime.UtcNow
        };

        await db.Categories.AddRangeAsync(bouquetsCategory, arrangementsCategory, singleStemsCategory);

        // 2. Seed Occasions
        var birthdayOccasion = new Occasion
        {
            Name = "Birthday",
            NameAr = "عيد ميلاد",
            CreatedAt = DateTime.UtcNow
        };

        var anniversaryOccasion = new Occasion
        {
            Name = "Anniversary",
            NameAr = "ذكرى سنوية",
            CreatedAt = DateTime.UtcNow
        };

        var valentinesOccasion = new Occasion
        {
            Name = "Valentine's Day",
            NameAr = "عيد الحب",
            CreatedAt = DateTime.UtcNow
        };

        await db.Occasions.AddRangeAsync(birthdayOccasion, anniversaryOccasion, valentinesOccasion);
        await db.SaveChangesAsync();

        // 3. Seed Products
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
            Category = bouquetsCategory,
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
            Category = bouquetsCategory,
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
            Description = "Lorem ipsum dolor sit amet consectetur. Id sit morbi ornare morbi duis rhoncus orci massa.",
            DescriptionAr = "وصف منتج تجريبي للباقة الوردية الأنيقة.",
            IsBestSeller = true,
            BestSellerOrder = 3,
            Category = bouquetsCategory,
            CreatedAt = DateTime.UtcNow,
            Images =
            [
                new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/103_1.jpg", SortOrder = 0 },
                new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/103_2.jpg", SortOrder = 1 },
                new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/103_3.jpg", SortOrder = 2 },
                new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/103_4.jpg", SortOrder = 3 }
            ],
            Includes =
            [
                new ProductInclude { Name = "Pink roses: 15", NameAr = "ورد وردي: ١٥" },
                new ProductInclude { Name = "White wrap", NameAr = "تغليف أبيض" }
            ]
        };

        var product4 = new Product
        {
            Name = "White Orchid Arrangement",
            NameAr = "تنسيقة أوركيد بيضاء",
            ImageUrl = "https://cdn.flowery-app.com/products/104.jpg",
            Currency = "EGP",
            Price = 1200,
            OriginalPrice = 1500,
            DiscountPercentage = 20,
            Status = ProductStatus.InStock,
            Description = "Elegant white orchid in a ceramic pot.",
            DescriptionAr = "أوركيد بيضاء أنيقة في أصيص خزفي.",
            IsBestSeller = false,
            BestSellerOrder = 0,
            Category = arrangementsCategory,
            CreatedAt = DateTime.UtcNow,
            Images =
            [
                new ProductImage { ImageUrl = "https://cdn.flowery-app.com/products/104_1.jpg", SortOrder = 0 }
            ],
            Includes =
            [
                new ProductInclude { Name = "White Orchid", NameAr = "أوركيد بيضاء" },
                new ProductInclude { Name = "Ceramic Pot", NameAr = "أصيص خزفي" }
            ]
        };

        await db.Products.AddRangeAsync(product1, product2, product3, product4);
        await db.SaveChangesAsync();

        // 4. Link Products & Occasions
        await db.ProductOccasions.AddRangeAsync(
            new ProductOccasion { ProductId = product1.Id, OccasionId = valentinesOccasion.Id },
            new ProductOccasion { ProductId = product1.Id, OccasionId = anniversaryOccasion.Id },
            new ProductOccasion { ProductId = product2.Id, OccasionId = birthdayOccasion.Id },
            new ProductOccasion { ProductId = product3.Id, OccasionId = valentinesOccasion.Id },
            new ProductOccasion { ProductId = product3.Id, OccasionId = birthdayOccasion.Id },
            new ProductOccasion { ProductId = product4.Id, OccasionId = anniversaryOccasion.Id }
        );

        await db.SaveChangesAsync();

        // 5. Seed Home Sections
        if (!await db.HomeSections.AnyAsync())
        {
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
                    OccasionId = valentinesOccasion.Id,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await db.SaveChangesAsync();
        }
    }
}
