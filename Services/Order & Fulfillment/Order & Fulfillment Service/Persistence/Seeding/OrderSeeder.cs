using Blocks.Contracts.Payment;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Persistence.Seeding;

public static class OrderSeeder
{
    public static async Task SeedAsync(FlowersOrderDbContext db)
    {
        // Real Store IDs from Address & Store Coverage Database
        var storeNasrCity = Guid.Parse("D3510586-76B8-474C-BC9A-079C4EB2DB0D");
        var storeDokki = Guid.Parse("01A0442B-0FEA-77AF-A582-6B886B5235FE");
        var storeMaadi = Guid.Parse("4CFEA377-F243-437D-A79F-9621C841B75A");
        var storeHeliopolis = Guid.Parse("456D88AE-769D-4054-B37F-9755F576AC40");

        var myDriverId = Guid.Parse("d0000000-0000-0000-0000-000000000001");
        var otherDriverId = Guid.Parse("d0000000-0000-0000-0000-000000000002");
        var customerId = Guid.Parse("c0000000-0000-0000-0000-000000000001");

        var order1Id = Guid.Parse("b1111111-1111-1111-1111-111111111111");
        var order2Id = Guid.Parse("b2222222-2222-2222-2222-222222222222");
        var order3Id = Guid.Parse("b3333333-3333-3333-3333-333333333333");
        var order4Id = Guid.Parse("b4444444-4444-4444-4444-444444444444");
        var order5Id = Guid.Parse("b5555555-5555-5555-5555-555555555555");

        var orders = new List<Order>
        {
            // Order 1: Unassigned, ready for pickup (Preparing) from Maadi Branch
            new()
            {
                Id = order1Id,
                CustomerId = customerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = storeMaadi,
                Status = OrderStatus.Preparing,
                PaymentMethod = PaymentMethod.Card,
                PaymentProvider = PaymentProvider.Paymob,
                Subtotal = 150.00m,
                DeliveryFee = 15.00m,
                Total = 165.00m,
                IsGift = false,
                RecipientName = "Sarah Ahmed",
                RecipientPhone = "+201001234567",
                AddressLine = "45 El-Nahr Street, Apartment 4B",
                City = "Cairo",
                Area = "Maadi",
                DeliveryLatitude = 29.9602,
                DeliveryLongitude = 31.2569,
                AssignedDriverId = null,
                AssignedAt = null,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow.AddMinutes(-45),
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order1Id,
                        ProductId = Guid.NewGuid(),
                        ProductName = "Red Roses Bouquet (12 Stems)",
                        Quantity = 1,
                        UnitPrice = 100.00m
                    },
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order1Id,
                        ProductId = Guid.NewGuid(),
                        ProductName = "Glass Flower Vase",
                        Quantity = 1,
                        UnitPrice = 50.00m
                    }
                ]
            },

            // Order 2: Another unassigned order (Preparing) from Nasr City Branch
            new()
            {
                Id = order2Id,
                CustomerId = customerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = storeNasrCity,
                Status = OrderStatus.Preparing,
                PaymentMethod = PaymentMethod.COD,
                PaymentProvider = null,
                Subtotal = 85.00m,
                DeliveryFee = 15.00m,
                Total = 100.00m,
                IsGift = false,
                RecipientName = "Omar Tarek",
                RecipientPhone = "+201123456789",
                AddressLine = "12 Abbas El Akkad St",
                City = "Cairo",
                Area = "Nasr City",
                DeliveryLatitude = 30.0511,
                DeliveryLongitude = 31.3656,
                AssignedDriverId = null,
                AssignedAt = null,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(3),
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order2Id,
                        ProductId = Guid.NewGuid(),
                        ProductName = "White Lilies Bouquet",
                        Quantity = 1,
                        UnitPrice = 85.00m
                    }
                ]
            },

            // Order 3: Unassigned gift order (Preparing) from Heliopolis Branch
            new()
            {
                Id = order3Id,
                CustomerId = customerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = storeHeliopolis,
                Status = OrderStatus.Preparing,
                PaymentMethod = PaymentMethod.Card,
                PaymentProvider = PaymentProvider.Paymob,
                Subtotal = 220.00m,
                DeliveryFee = 15.00m,
                Total = 235.00m,
                IsGift = true,
                RecipientName = "Karim Mostafa",
                RecipientPhone = "+201011122233",
                GiftRecipientName = "Laila Hassan",
                GiftRecipientPhone = "+201098765432",
                AddressLine = "7 Cleopatra Street, Korba",
                City = "Cairo",
                Area = "Heliopolis",
                DeliveryLatitude = 30.0866,
                DeliveryLongitude = 31.3225,
                AssignedDriverId = null,
                AssignedAt = null,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(4),
                CreatedAt = DateTime.UtcNow.AddMinutes(-15),
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order3Id,
                        ProductId = Guid.NewGuid(),
                        ProductName = "Luxury Orchid Arrangement",
                        Quantity = 1,
                        UnitPrice = 190.00m
                    },
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order3Id,
                        ProductId = Guid.NewGuid(),
                        ProductName = "Personalized Greeting Card",
                        Quantity = 1,
                        UnitPrice = 30.00m
                    }
                ]
            },

            // Order 4: Delivered order assigned to our test driver from Dokki Hub (History)
            new()
            {
                Id = order4Id,
                CustomerId = customerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = storeDokki,
                Status = OrderStatus.Delivered,
                PaymentMethod = PaymentMethod.Card,
                PaymentProvider = PaymentProvider.Paymob,
                Subtotal = 120.00m,
                DeliveryFee = 15.00m,
                Total = 135.00m,
                IsGift = false,
                RecipientName = "Mona Zaki",
                RecipientPhone = "+201200112233",
                AddressLine = "88 El-Thawra St",
                City = "Cairo",
                Area = "Heliopolis",
                DeliveryLatitude = 30.0890,
                DeliveryLongitude = 31.3250,
                AssignedDriverId = myDriverId,
                AssignedAt = DateTime.UtcNow.AddHours(-3),
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(-2),
                CreatedAt = DateTime.UtcNow.AddHours(-4),
                UpdatedAt = DateTime.UtcNow.AddHours(-1),
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order4Id,
                        ProductId = Guid.NewGuid(),
                        ProductName = "Pink Tulips Bunch",
                        Quantity = 1,
                        UnitPrice = 120.00m
                    }
                ]
            },

            // Order 5: Order assigned to another driver from Nasr City Branch (Negative test 403/409)
            new()
            {
                Id = order5Id,
                CustomerId = customerId,
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = storeNasrCity,
                Status = OrderStatus.PickedUp,
                PaymentMethod = PaymentMethod.COD,
                PaymentProvider = null,
                Subtotal = 75.00m,
                DeliveryFee = 15.00m,
                Total = 90.00m,
                IsGift = false,
                RecipientName = "Tarek Youssef",
                RecipientPhone = "+201500998877",
                AddressLine = "21 Degla Street",
                City = "Cairo",
                Area = "Maadi",
                DeliveryLatitude = 29.9620,
                DeliveryLongitude = 31.2580,
                AssignedDriverId = otherDriverId,
                AssignedAt = DateTime.UtcNow.AddMinutes(-30),
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow.AddHours(-1),
            // ── Orders for GetOrders Task (TestCustomerId) ──
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000001"),
                CustomerId = Guid.Parse("01a07222-e672-7703-bbb1-3653edec4f93"),
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Delivered,
                PaymentMethod = PaymentMethod.Card,
                PaymentProvider = PaymentProvider.Paymob,
                Subtotal = 350.00m,
                DeliveryFee = 25.00m,
                Total = 375.00m,
                IsGift = false,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(-2),
                RecipientName = "Norhan Ahmed",
                RecipientPhone = "+201012345678",
                AddressLine = "15 Nile Street, Dokki",
                City = "Giza",
                Area = "Dokki",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
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
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000002"),
                CustomerId = Guid.Parse("01a07222-e672-7703-bbb1-3653edec4f93"),
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
                EstimatedDeliveryAt = DateTime.UtcNow.AddMinutes(30),
                RecipientName = "Sarah Mohamed",
                RecipientPhone = "+201098765432",
                AddressLine = "8 Tahrir Street, Downtown",
                City = "Cairo",
                Area = "Downtown",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
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
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000003"),
                CustomerId = Guid.Parse("01a07222-e672-7703-bbb1-3653edec4f93"),
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Preparing,
                PaymentMethod = PaymentMethod.Card,
                PaymentProvider = PaymentProvider.Stripe,
                Subtotal = 520.00m,
                DeliveryFee = 0.00m,
                Total = 520.00m,
                IsGift = false,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(2),
                RecipientName = "Norhan Ahmed",
                RecipientPhone = "+201012345678",
                AddressLine = "22 Mostafa Kamel Street, Smouha",
                City = "Alexandria",
                Area = "Smouha",
                CreatedAt = DateTime.UtcNow.AddMinutes(-45),
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
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000004"),
                CustomerId = Guid.Parse("01a07222-e672-7703-bbb1-3653edec4f93"),
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Cancelled,
                PaymentMethod = PaymentMethod.COD,
                Subtotal = 150.00m,
                DeliveryFee = 20.00m,
                Total = 170.00m,
                IsGift = false,
                EstimatedDeliveryAt = DateTime.UtcNow.AddDays(-1),
                CancellationReason = "Customer cancelled — incorrect address",
                RecipientName = "Norhan Ahmed",
                RecipientPhone = "+201012345678",
                AddressLine = "5 Gameat El Dewal Street, Mohandessin",
                City = "Giza",
                Area = "Mohandessin",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
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
            new()
            {
                Id = Guid.Parse("aaaa1111-0001-0001-0001-000000000005"),
                CustomerId = Guid.Parse("01a07222-e672-7703-bbb1-3653edec4f93"),
                CartId = Guid.NewGuid(),
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Placed,
                PaymentMethod = PaymentMethod.Card,
                PaymentProvider = PaymentProvider.Paymob,
                Subtotal = 430.00m,
                DeliveryFee = 15.00m,
                Total = 445.00m,
                IsGift = true,
                GiftRecipientName = "Ahmed Khaled",
                GiftRecipientPhone = "+201155443322",
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(3),
                RecipientName = "Ahmed Khaled",
                RecipientPhone = "+201155443322",
                AddressLine = "12 Horreya Street, Heliopolis",
                City = "Cairo",
                Area = "Heliopolis",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
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

        // Idempotent insertion: only insert orders that do not already exist
        var newOrders = new List<Order>();
        foreach (var order in orders)
        {
            var exists = await db.Orders.IgnoreQueryFilters().AnyAsync(o => o.Id == order.Id);
            if (!exists)
            {
                newOrders.Add(order);
            }
        }

        if (newOrders.Count > 0)
        {
            db.Orders.AddRange(newOrders);
            await db.SaveChangesAsync();
        }
    }
}
