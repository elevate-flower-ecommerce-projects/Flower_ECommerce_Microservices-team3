using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Persistence.Seeding;

public static class CatalogDataSeeder
{
    public static async Task SeedAsync(FlowersCatalogDbContext db)
    {
        // 1. Seed Categories if empty
        var rosesCategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var tulipsCategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var bouquetsCategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        var categories = new List<Category>
        {
            new()
            {
                Id = rosesCategoryId,
                Name = "Roses",
                NameAr = "ورود",
                Icon = "roses.png",
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = tulipsCategoryId,
                Name = "Tulips",
                NameAr = "توليب",
                Icon = "tulips.png",
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = bouquetsCategoryId,
                Name = "Bouquets",
                NameAr = "باقات زهور",
                Icon = "bouquets.png",
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var category in categories)
        {
            var exists = await db.Categories.IgnoreQueryFilters().AnyAsync(c => c.Id == category.Id || c.Name == category.Name);
            if (!exists)
            {
                await db.Categories.AddAsync(category);
            }
        }
        await db.SaveChangesAsync();

        // 2. Seed Occasions if empty
        var birthdayOccasionId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var anniversaryOccasionId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var valentinesOccasionId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        var occasions = new List<Occasion>
        {
            new()
            {
                Id = birthdayOccasionId,
                Name = "Birthday",
                NameAr = "عيد ميلاد",
                ImageUrl = "https://images.unsplash.com/photo-1558636508-e0db3814bd1d?auto=format&fit=crop&w=800&q=80",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = anniversaryOccasionId,
                Name = "Anniversary",
                NameAr = "ذكرى سنوية",
                ImageUrl = "https://images.unsplash.com/photo-1515934751635-c81c6bc9a2d8?auto=format&fit=crop&w=800&q=80",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = valentinesOccasionId,
                Name = "Valentine's Day",
                NameAr = "عيد الحب",
                ImageUrl = "https://images.unsplash.com/photo-1518199266791-5375a83190b7?auto=format&fit=crop&w=800&q=80",
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var occasion in occasions)
        {
            var exists = await db.Occasions.IgnoreQueryFilters().AnyAsync(o => o.Id == occasion.Id || o.Name == occasion.Name);
            if (!exists)
            {
                await db.Occasions.AddAsync(occasion);
            }
        }
        await db.SaveChangesAsync();

        // Retrieve actual category and occasion entities
        var rosesCat = await db.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Name == "Roses") ?? categories[0];
        var tulipsCat = await db.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Name == "Tulips") ?? categories[1];
        var bouquetsCat = await db.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Name == "Bouquets") ?? categories[2];

        var bdayOcc = await db.Occasions.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Name.Contains("Birthday")) ?? occasions[0];
        var annOcc = await db.Occasions.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Name.Contains("Anniversary")) ?? occasions[1];
        var valOcc = await db.Occasions.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Name.Contains("Valentine")) ?? occasions[2];

        // 3. Seed Products for each category
        var productsToSeed = new List<(Product product, List<Guid> occasionIds)>
        {
            // ─── Roses Category ───
            (
                new Product
                {
                    Name = "Red Velvet Roses",
                    NameAr = "ورود المخمل الأحمر",
                    ImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23?auto=format&fit=crop&w=800&q=80",
                    Currency = "EGP",
                    Price = 750,
                    OriginalPrice = 900,
                    DiscountPercentage = 16,
                    Status = ProductStatus.InStock,
                    Description = "Premium hand-picked red velvet roses arranged to perfection.",
                    DescriptionAr = "ورود مخملية حمراء منتقاة بعناية ومقدمة بتنسيق فاخر.",
                    IsBestSeller = true,
                    BestSellerOrder = 1,
                    CategoryId = rosesCat.Id,
                    CreatedAt = DateTime.UtcNow,
                    Images = [new() { ImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23?auto=format&fit=crop&w=800&q=80", SortOrder = 0 }],
                    Includes = [new() { Name = "12 Red Roses", NameAr = "١٢ وردة حمراء" }, new() { Name = "Luxury Ribbon", NameAr = "شريط فاخر" }]
                },
                [valOcc.Id, annOcc.Id]
            ),
            (
                new Product
                {
                    Name = "White Elegance Roses",
                    NameAr = "ورود بيضاء أنيقة",
                    ImageUrl = "https://images.unsplash.com/photo-1533616688419-b7a58556458e?auto=format&fit=crop&w=800&q=80",
                    Currency = "EGP",
                    Price = 850,
                    OriginalPrice = null,
                    DiscountPercentage = null,
                    Status = ProductStatus.InStock,
                    Description = "Pure white roses symbolizing grace, purity, and sophistication.",
                    DescriptionAr = "ورود بيضاء نقية ترمز إلى الأناقة والرقي.",
                    IsBestSeller = false,
                    BestSellerOrder = 0,
                    CategoryId = rosesCat.Id,
                    CreatedAt = DateTime.UtcNow,
                    Images = [new() { ImageUrl = "https://images.unsplash.com/photo-1533616688419-b7a58556458e?auto=format&fit=crop&w=800&q=80", SortOrder = 0 }],
                    Includes = [new() { Name = "15 White Roses", NameAr = "١٥ وردة بيضاء" }]
                },
                [annOcc.Id, bdayOcc.Id]
            ),
            (
                new Product
                {
                    Name = "Yellow Sunshine Roses",
                    NameAr = "ورود صفراء مشرقة",
                    ImageUrl = "https://images.unsplash.com/photo-1563241527-3004b7be0ffd?auto=format&fit=crop&w=800&q=80",
                    Currency = "EGP",
                    Price = 650,
                    OriginalPrice = null,
                    DiscountPercentage = null,
                    Status = ProductStatus.InStock,
                    Description = "Bright and radiant yellow roses that spread warmth and joy.",
                    DescriptionAr = "ورود صفراء مشرقة تنشر البهجة والدفء في كل مناسبة.",
                    IsBestSeller = false,
                    BestSellerOrder = 0,
                    CategoryId = rosesCat.Id,
                    CreatedAt = DateTime.UtcNow,
                    Images = [new() { ImageUrl = "https://images.unsplash.com/photo-1563241527-3004b7be0ffd?auto=format&fit=crop&w=800&q=80", SortOrder = 0 }],
                    Includes = [new() { Name = "10 Yellow Roses", NameAr = "١٠ ورود صفراء" }]
                },
                [bdayOcc.Id]
            ),

            // ─── Tulips Category ───
            (
                new Product
                {
                    Name = "Royal Purple Tulips",
                    NameAr = "توليب بنفسجي ملكي",
                    ImageUrl = "https://images.unsplash.com/photo-1520763185298-1b434c919102?auto=format&fit=crop&w=800&q=80",
                    Currency = "EGP",
                    Price = 850,
                    OriginalPrice = 1000,
                    DiscountPercentage = 15,
                    Status = ProductStatus.InStock,
                    Description = "Enchanting purple Dutch tulips arranged for royal celebrations.",
                    DescriptionAr = "أزهار توليب بنفسجية ساحرة مستوردة للاحتفالات الراقية.",
                    IsBestSeller = true,
                    BestSellerOrder = 2,
                    CategoryId = tulipsCat.Id,
                    CreatedAt = DateTime.UtcNow,
                    Images = [new() { ImageUrl = "https://images.unsplash.com/photo-1520763185298-1b434c919102?auto=format&fit=crop&w=800&q=80", SortOrder = 0 }],
                    Includes = [new() { Name = "15 Purple Tulips", NameAr = "١٥ زهرة توليب بنفسجي" }]
                },
                [bdayOcc.Id, valOcc.Id]
            ),
            (
                new Product
                {
                    Name = "Pure White Dutch Tulips",
                    NameAr = "توليب هولندي أبيض",
                    ImageUrl = "https://images.unsplash.com/photo-1589244159943-460088ed5c92?auto=format&fit=crop&w=800&q=80",
                    Currency = "EGP",
                    Price = 900,
                    OriginalPrice = null,
                    DiscountPercentage = null,
                    Status = ProductStatus.InStock,
                    Description = "Fresh white tulips representing innocence and peace.",
                    DescriptionAr = "توليب أبيض نضر يجسد النقاء والهدوء.",
                    IsBestSeller = false,
                    BestSellerOrder = 0,
                    CategoryId = tulipsCat.Id,
                    CreatedAt = DateTime.UtcNow,
                    Images = [new() { ImageUrl = "https://images.unsplash.com/photo-1589244159943-460088ed5c92?auto=format&fit=crop&w=800&q=80", SortOrder = 0 }],
                    Includes = [new() { Name = "12 White Tulips", NameAr = "١٢ زهرة توليب أبيض" }]
                },
                [annOcc.Id]
            ),
            (
                new Product
                {
                    Name = "Sunset Orange Tulips",
                    NameAr = "توليب برتقالي بلون الغروب",
                    ImageUrl = "https://images.unsplash.com/photo-1518895949257-7621c3c786d7?auto=format&fit=crop&w=800&q=80",
                    Currency = "EGP",
                    Price = 750,
                    OriginalPrice = null,
                    DiscountPercentage = null,
                    Status = ProductStatus.InStock,
                    Description = "Vibrant orange tulips with warm tones reminiscent of golden sunsets.",
                    DescriptionAr = "توليب برتقالي زاهٍ بألوان دافئة تحاكي غروب الشمس.",
                    IsBestSeller = false,
                    BestSellerOrder = 0,
                    CategoryId = tulipsCat.Id,
                    CreatedAt = DateTime.UtcNow,
                    Images = [new() { ImageUrl = "https://images.unsplash.com/photo-1518895949257-7621c3c786d7?auto=format&fit=crop&w=800&q=80", SortOrder = 0 }],
                    Includes = [new() { Name = "10 Orange Tulips", NameAr = "١٠ زهور توليب برتقالي" }]
                },
                [bdayOcc.Id]
            ),

            // ─── Bouquets Category ───
            (
                new Product
                {
                    Name = "15 Pink Rose Bouquet",
                    NameAr = "باقة ١٥ وردة وردية",
                    ImageUrl = "https://images.unsplash.com/photo-1561181286-d3fee7d55364?auto=format&fit=crop&w=800&q=80",
                    Currency = "EGP",
                    Price = 1500,
                    OriginalPrice = null,
                    DiscountPercentage = null,
                    Status = ProductStatus.InStock,
                    Description = "Fresh fragrant roses arranged into a lovely bouquet.",
                    DescriptionAr = "باقة أنيقة من الورود الوردية الفواحة.",
                    IsBestSeller = true,
                    BestSellerOrder = 3,
                    CategoryId = bouquetsCat.Id,
                    CreatedAt = DateTime.UtcNow,
                    Images = [new() { ImageUrl = "https://images.unsplash.com/photo-1561181286-d3fee7d55364?auto=format&fit=crop&w=800&q=80", SortOrder = 0 }],
                    Includes = [new() { Name = "15 Pink Roses", NameAr = "١٥ وردة وردية" }, new() { Name = "White wrap", NameAr = "تغليف أبيض" }]
                },
                [valOcc.Id, bdayOcc.Id]
            ),
            (
                new Product
                {
                    Name = "Sunny Sunflower Bouquet",
                    NameAr = "باقة دوار الشمس المشرقة",
                    ImageUrl = "https://images.unsplash.com/photo-1597848212624-a19eb35e2651?auto=format&fit=crop&w=800&q=80",
                    Currency = "EGP",
                    Price = 600,
                    OriginalPrice = null,
                    DiscountPercentage = null,
                    Status = ProductStatus.InStock,
                    Description = "Bright yellow sunflowers that bring warmth and joy.",
                    DescriptionAr = "دوار شمس أصفر مشرق يبعث الدفء والبهجة.",
                    IsBestSeller = true,
                    BestSellerOrder = 4,
                    CategoryId = bouquetsCat.Id,
                    CreatedAt = DateTime.UtcNow,
                    Images = [new() { ImageUrl = "https://images.unsplash.com/photo-1597848212624-a19eb35e2651?auto=format&fit=crop&w=800&q=80", SortOrder = 0 }],
                    Includes = [new() { Name = "Sunflowers", NameAr = "عباد الشمس" }, new() { Name = "Yellow Ribbon", NameAr = "شريط أصفر" }]
                },
                [bdayOcc.Id]
            ),
            (
                new Product
                {
                    Name = "Luxury Mixed Floral Bouquet",
                    NameAr = "باقة الزهور المشكلة الفاخرة",
                    ImageUrl = "https://images.unsplash.com/photo-1508610048659-a06b669e3321?auto=format&fit=crop&w=800&q=80",
                    Currency = "EGP",
                    Price = 1800,
                    OriginalPrice = 2200,
                    DiscountPercentage = 18,
                    Status = ProductStatus.InStock,
                    Description = "A masterfully blended arrangement of premium exotic flowers and greens.",
                    DescriptionAr = "تشكيلة استثنائية من أرقى الزهور الطبيعية والخضار النضر.",
                    IsBestSeller = false,
                    BestSellerOrder = 0,
                    CategoryId = bouquetsCat.Id,
                    CreatedAt = DateTime.UtcNow,
                    Images = [new() { ImageUrl = "https://images.unsplash.com/photo-1508610048659-a06b669e3321?auto=format&fit=crop&w=800&q=80", SortOrder = 0 }],
                    Includes = [new() { Name = "Mixed Luxury Flowers", NameAr = "زهور مشكلة فاخرة" }, new() { Name = "Designer Vase", NameAr = "فازة مميزة" }]
                },
                [annOcc.Id, valOcc.Id]
            )
        };

        foreach (var (prod, occIds) in productsToSeed)
        {
            var existingProduct = await db.Products.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Name == prod.Name);
            if (existingProduct == null)
            {
                await db.Products.AddAsync(prod);
                await db.SaveChangesAsync();

                foreach (var occId in occIds)
                {
                    var hasLink = await db.ProductOccasions.IgnoreQueryFilters().AnyAsync(po => po.ProductId == prod.Id && po.OccasionId == occId);
                    if (!hasLink)
                    {
                        await db.ProductOccasions.AddAsync(new ProductOccasion { ProductId = prod.Id, OccasionId = occId });
                    }
                }
                await db.SaveChangesAsync();
            }
        }

        // 4. Seed Home Sections if empty
        if (!await db.HomeSections.IgnoreQueryFilters().AnyAsync())
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
                    OccasionId = valOcc.Id,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await db.SaveChangesAsync();
        }
    }
}