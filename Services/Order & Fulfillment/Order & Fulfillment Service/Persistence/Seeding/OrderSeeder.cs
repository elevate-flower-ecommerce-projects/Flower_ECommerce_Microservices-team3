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
                PaymentGateway = PaymentGateway.Stripe,
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
                PaymentGateway = null,
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
                PaymentGateway = PaymentGateway.Stripe,
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
                PaymentGateway = PaymentGateway.Stripe,
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
                PaymentGateway = null,
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
                Items =
                [
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order5Id,
                        ProductId = Guid.NewGuid(),
                        ProductName = "Sunflower Basket",
                        Quantity = 1,
                        UnitPrice = 75.00m
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
