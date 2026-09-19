using Blocks.Contracts.Payment;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Persistence.Seeding;

public static class FlowersOrderSeeder
{
    public static async Task SeedAsync(FlowersOrderDbContext context)
    {
        // ── Fixed IDs matching Identity Service (FixedTestUsersSeeder) ───
        var customer1Id = Guid.Parse("01a08dfc-3620-71ae-8ac5-3e8c7b3f3c8b"); // Nourhan
        var customer2Id = Guid.Parse("01a08dfc-3620-71ae-8ac5-3e8c7b3f3c8c"); // Layla

        var driver1Id   = Guid.Parse("01a03975-5bd9-7db4-8e83-599af35ee26c"); // Karim Driver
        var driver2Id   = Guid.Parse("01a03975-5bd9-7db4-8e83-599af35ee26d"); // Ahmed Driver

        // ── Order IDs ────────────────────────────────────────────────────
        var outForDeliveryOrderId   = Guid.Parse("bbbb2222-0002-0002-0002-000000000002");
        var awaitingConfirmOrderId  = Guid.Parse("bbbb2222-0002-0002-0002-000000000003");
        var cancelledOrderId        = Guid.Parse("bbbb2222-0002-0002-0002-000000000004");
        var preparingOrderId        = Guid.Parse("bbbb2222-0002-0002-0002-000000000005");
        var customer2OrderId        = Guid.Parse("bbbb2222-0002-0002-0002-000000000006");

        // ── 1. OutForDelivery (Customer 1 + Driver 1 Karim) ─────────────
        var outForDeliveryOrder = await context.Orders.FirstOrDefaultAsync(o => o.Id == outForDeliveryOrderId);
        if (outForDeliveryOrder == null)
        {
            outForDeliveryOrder = new Order
            {
                Id = outForDeliveryOrderId,
                CustomerId = customer1Id,
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.OutForDelivery,
                PaymentMethod = PaymentMethod.Card,
                Subtotal = 100,
                DeliveryFee = 15,
                Total = 115,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(1),
                RecipientName = "Nourhan Eng",
                RecipientPhone = "01099990001",
                AddressLine = "123 Tahrir Street",
                City = "Cairo",
                Area = "Downtown",
                DeliveryLatitude = 30.0444,
                DeliveryLongitude = 31.2357,
                AssignedDriverId = driver1Id,
                AssignedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            await context.Orders.AddAsync(outForDeliveryOrder);
        }
        else
        {
            outForDeliveryOrder.CustomerId = customer1Id;
            outForDeliveryOrder.AssignedDriverId = driver1Id;
            outForDeliveryOrder.Status = OrderStatus.OutForDelivery;
            outForDeliveryOrder.DriverName = null;
            outForDeliveryOrder.DriverPhone = null;
            outForDeliveryOrder.DriverPhotoUrl = null;
        }

        // ── 2. AwaitingDeliveryConfirmation (Customer 1 + Driver 2 Ahmed) ─
        var awaitingOrder = await context.Orders.FirstOrDefaultAsync(o => o.Id == awaitingConfirmOrderId);
        if (awaitingOrder == null)
        {
            awaitingOrder = new Order
            {
                Id = awaitingConfirmOrderId,
                CustomerId = customer1Id,
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.AwaitingDeliveryConfirmation,
                PaymentMethod = PaymentMethod.COD,
                Subtotal = 80,
                DeliveryFee = 15,
                Total = 95,
                EstimatedDeliveryAt = DateTime.UtcNow.AddMinutes(-10),
                RecipientName = "Nourhan Eng",
                RecipientPhone = "01099990001",
                AddressLine = "123 Tahrir Street",
                City = "Cairo",
                Area = "Downtown",
                DeliveryLatitude = 30.0444,
                DeliveryLongitude = 31.2357,
                AssignedDriverId = driver2Id,
                AssignedAt = DateTime.UtcNow.AddHours(-1),
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            };
            await context.Orders.AddAsync(awaitingOrder);
        }
        else
        {
            awaitingOrder.CustomerId = customer1Id;
            awaitingOrder.AssignedDriverId = driver2Id;
            awaitingOrder.Status = OrderStatus.AwaitingDeliveryConfirmation;
            awaitingOrder.DriverName = null;
            awaitingOrder.DriverPhone = null;
            awaitingOrder.DriverPhotoUrl = null;
        }

        // ── 3. Cancelled (Customer 1) ───────────────────────────────────
        var cancelledOrder = await context.Orders.FirstOrDefaultAsync(o => o.Id == cancelledOrderId);
        if (cancelledOrder == null)
        {
            cancelledOrder = new Order
            {
                Id = cancelledOrderId,
                CustomerId = customer1Id,
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Cancelled,
                PaymentMethod = PaymentMethod.Card,
                Subtotal = 60,
                DeliveryFee = 15,
                Total = 75,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(1),
                RecipientName = "Nourhan Eng",
                RecipientPhone = "01099990001",
                AddressLine = "123 Tahrir Street",
                City = "Cairo",
                Area = "Downtown",
                DeliveryLatitude = 30.0444,
                DeliveryLongitude = 31.2357,
                CancellationReason = "Customer changed their mind",
                AssignedDriverId = null,
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            };
            await context.Orders.AddAsync(cancelledOrder);
        }
        else
        {
            cancelledOrder.CustomerId = customer1Id;
            cancelledOrder.Status = OrderStatus.Cancelled;
        }

        // ── 4. Preparing (Customer 1 — No Driver Yet) ────────────────────
        var preparingOrder = await context.Orders.FirstOrDefaultAsync(o => o.Id == preparingOrderId);
        if (preparingOrder == null)
        {
            preparingOrder = new Order
            {
                Id = preparingOrderId,
                CustomerId = customer1Id,
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Preparing,
                PaymentMethod = PaymentMethod.Card,
                Subtotal = 120,
                DeliveryFee = 15,
                Total = 135,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(2),
                RecipientName = "Nourhan Eng",
                RecipientPhone = "01099990001",
                AddressLine = "123 Tahrir Street",
                City = "Cairo",
                Area = "Downtown",
                DeliveryLatitude = 30.0444,
                DeliveryLongitude = 31.2357,
                AssignedDriverId = null,
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            };
            await context.Orders.AddAsync(preparingOrder);
        }
        else
        {
            preparingOrder.CustomerId = customer1Id;
            preparingOrder.Status = OrderStatus.Preparing;
        }

        // ── 5. Delivered (Customer 2 Layla + Driver 2 Ahmed) ─────────────
        var customer2Order = await context.Orders.FirstOrDefaultAsync(o => o.Id == customer2OrderId);
        if (customer2Order == null)
        {
            customer2Order = new Order
            {
                Id = customer2OrderId,
                CustomerId = customer2Id,
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.Delivered,
                PaymentMethod = PaymentMethod.Card,
                Subtotal = 150,
                DeliveryFee = 20,
                Total = 170,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(-1),
                RecipientName = "Layla Hassan",
                RecipientPhone = "01099990002",
                AddressLine = "456 Nasr City Street",
                City = "Cairo",
                Area = "Nasr City",
                DeliveryLatitude = 30.0566,
                DeliveryLongitude = 31.3301,
                AssignedDriverId = driver2Id,
                AssignedAt = DateTime.UtcNow.AddHours(-2),
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            };
            await context.Orders.AddAsync(customer2Order);
        }
        else
        {
            customer2Order.CustomerId = customer2Id;
            customer2Order.AssignedDriverId = driver2Id;
            customer2Order.Status = OrderStatus.Delivered;
            customer2Order.DriverName = null;
            customer2Order.DriverPhone = null;
            customer2Order.DriverPhotoUrl = null;
        }

        await context.SaveChangesAsync();

        // ── Driver Location for Driver 1 (Karim) ─────────────────────────
        var karimLocation = await context.DriverLocations
            .FirstOrDefaultAsync(l => l.DriverId == driver1Id);

        if (karimLocation == null)
        {
            await context.DriverLocations.AddAsync(new DriverLocation
            {
                Id = Guid.NewGuid(),
                DriverId = driver1Id,
                ActiveOrderId = outForDeliveryOrderId,
                Lat = 30.0500,
                Lng = 31.2400,
                RecordedAt = DateTime.UtcNow
            });
        }
        else
        {
            karimLocation.ActiveOrderId = outForDeliveryOrderId;
            karimLocation.Lat = 30.0500;
            karimLocation.Lng = 31.2400;
            karimLocation.RecordedAt = DateTime.UtcNow;
        }

        // ── Driver Location for Driver 2 (Ahmed) ─────────────────────────
        var ahmedLocation = await context.DriverLocations
            .FirstOrDefaultAsync(l => l.DriverId == driver2Id);

        if (ahmedLocation == null)
        {
            await context.DriverLocations.AddAsync(new DriverLocation
            {
                Id = Guid.NewGuid(),
                DriverId = driver2Id,
                ActiveOrderId = customer2OrderId,
                Lat = 30.0600,
                Lng = 31.3200,
                RecordedAt = DateTime.UtcNow
            });
        }
        else
        {
            ahmedLocation.ActiveOrderId = customer2OrderId;
            ahmedLocation.Lat = 30.0600;
            ahmedLocation.Lng = 31.3200;
            ahmedLocation.RecordedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }
}
