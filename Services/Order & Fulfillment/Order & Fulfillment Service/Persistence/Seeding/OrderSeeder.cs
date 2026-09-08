using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Persistence.Seeding;

public static class OrderSeeder
{
    // Fixed CustomerId for testing — use the same ID when generating a JWT token
    private static readonly Guid TestCustomerId = Guid.Parse("01a07222-e672-7703-bbb1-3653edec4f93");

    public static async Task SeedAsync(FlowersOrderDbContext context)
    {
        if (await context.Orders.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var orders = new List<Order>
        {
            // ── Order 1: Delivered (completed order) ──
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000001"),
                CustomerId = TestCustomerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Delivered,
                PaymentMethod = PaymentMethod.Card,
                PaymentGateway = PaymentGateway.Paymob,
                Subtotal = 350.00m,
                DeliveryFee = 25.00m,
                Total = 375.00m,
                IsGift = false,
                EstimatedDeliveryAt = now.AddHours(-2),
                RecipientName = "Norhan Ahmed",
                RecipientPhone = "+201012345678",
                AddressLine = "15 Nile Street, Dokki",
                City = "Giza",
                Area = "Dokki",
                CreatedAt = now.AddDays(-3),
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "Classic Red Rose Bouquet",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1490750967868-88aa4f44baee?w=200",
                        Quantity = 1,
                        UnitPrice = 250.00m,
                    },
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "Premium Belgian Chocolate Box",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1549007994-cb92caebd54b?w=200",
                        Quantity = 2,
                        UnitPrice = 50.00m,
                    }
                ]
            },

            // ── Order 2: OutForDelivery (active delivery) ──
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000002"),
                CustomerId = TestCustomerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.OutForDelivery,
                PaymentMethod = PaymentMethod.COD,
                Subtotal = 180.00m,
                DeliveryFee = 30.00m,
                Total = 210.00m,
                IsGift = true,
                GiftRecipientName = "Sarah Mohamed",
                GiftRecipientPhone = "+201098765432",
                EstimatedDeliveryAt = now.AddMinutes(30),
                RecipientName = "Sarah Mohamed",
                RecipientPhone = "+201098765432",
                AddressLine = "8 Tahrir Street, Downtown",
                City = "Cairo",
                Area = "Downtown",
                CreatedAt = now.AddHours(-1),
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "Colorful Tulip Bouquet",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1524386416438-98b9b2d4b433?w=200",
                        Quantity = 1,
                        UnitPrice = 180.00m,
                    }
                ]
            },

            // ── Order 3: Preparing (being prepared at the store) ──
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000003"),
                CustomerId = TestCustomerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Preparing,
                PaymentMethod = PaymentMethod.Card,
                PaymentGateway = PaymentGateway.Stripe,
                Subtotal = 520.00m,
                DeliveryFee = 0.00m,
                Total = 520.00m,
                IsGift = false,
                EstimatedDeliveryAt = now.AddHours(2),
                RecipientName = "Norhan Ahmed",
                RecipientPhone = "+201012345678",
                AddressLine = "22 Mostafa Kamel Street, Smouha",
                City = "Alexandria",
                Area = "Smouha",
                CreatedAt = now.AddMinutes(-45),
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "Sunflower Bouquet",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1597848212624-a19eb35e2651?w=200",
                        Quantity = 2,
                        UnitPrice = 200.00m,
                    },
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "Birthday Helium Balloon",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1530103862676-de8c9debad1d?w=200",
                        Quantity = 1,
                        UnitPrice = 120.00m,
                    }
                ]
            },

            // ── Order 4: Cancelled ──
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000004"),
                CustomerId = TestCustomerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Cancelled,
                PaymentMethod = PaymentMethod.COD,
                Subtotal = 150.00m,
                DeliveryFee = 20.00m,
                Total = 170.00m,
                IsGift = false,
                EstimatedDeliveryAt = now.AddDays(-1),
                CancellationReason = "Customer cancelled — incorrect address",
                RecipientName = "Norhan Ahmed",
                RecipientPhone = "+201012345678",
                AddressLine = "5 Gameat El Dewal Street, Mohandessin",
                City = "Giza",
                Area = "Mohandessin",
                CreatedAt = now.AddDays(-5),
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "White Rose Bouquet",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1487530811176-3780de880c2d?w=200",
                        Quantity = 1,
                        UnitPrice = 150.00m,
                    }
                ]
            },

            // ── Order 5: Placed (just placed, pending preparation) ──
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000005"),
                CustomerId = TestCustomerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Placed,
                PaymentMethod = PaymentMethod.Card,
                PaymentGateway = PaymentGateway.Paymob,
                Subtotal = 430.00m,
                DeliveryFee = 15.00m,
                Total = 445.00m,
                IsGift = true,
                GiftRecipientName = "Ahmed Khaled",
                GiftRecipientPhone = "+201155443322",
                EstimatedDeliveryAt = now.AddHours(3),
                RecipientName = "Ahmed Khaled",
                RecipientPhone = "+201155443322",
                AddressLine = "12 Horreya Street, Heliopolis",
                City = "Cairo",
                Area = "Heliopolis",
                CreatedAt = now.AddMinutes(-10),
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "Premium Lily Bouquet",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1468327768560-75b778cbb551?w=200",
                        Quantity = 1,
                        UnitPrice = 350.00m,
                    },
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "Handwritten Greeting Card",
                        ThumbnailUrl = null,
                        Quantity = 1,
                        UnitPrice = 80.00m,
                    }
                ]
            }
        };

        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();
    }
}
