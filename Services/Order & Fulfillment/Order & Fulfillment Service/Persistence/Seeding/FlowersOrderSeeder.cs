using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Persistence.Seeding;

public static class FlowersOrderSeeder
{
    public static async Task SeedAsync(FlowersOrderDbContext context)
    {
        var karimDriverId = Guid.Parse("01a03975-5bd9-7db4-8e83-599af35ee26c");
        var customerId = Guid.Parse("01a08dfc-3620-71ae-8ac5-3e8c7b3f3c8b");
        var testOrderId = Guid.Parse("bbbb2222-0002-0002-0002-000000000002");

        // 1. Seed or Update the test order
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == testOrderId);

        if (order == null)
        {
            order = new Order
            {
                Id = testOrderId,
                CustomerId = customerId,
                AddressId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = OrderStatus.OutForDelivery,
                PaymentMethod = PaymentMethod.Card,
                Subtotal = 100,
                DeliveryFee = 15,
                Total = 115,
                EstimatedDeliveryAt = DateTime.UtcNow.AddHours(1),
                RecipientName = "Nourhan Eng",
                RecipientPhone = "01099999999",
                AddressLine = "123 Tahrir Street",
                City = "Cairo",
                Area = "Downtown",
                DeliveryLatitude = 30.0444,
                DeliveryLongitude = 31.2357,
                AssignedDriverId = karimDriverId,
                AssignedAt = DateTime.UtcNow,
                DriverName = null,
                DriverPhone = null,
                CreatedAt = DateTime.UtcNow
            };

            await context.Orders.AddAsync(order);
        }
        else
        {
            order.CustomerId = customerId;
            order.AssignedDriverId = karimDriverId;
            order.DriverName = null;
            order.DriverPhone = null;
            order.Status = OrderStatus.OutForDelivery;
        }

        // 2. Seed live location for Karim Driver (unique index is on DriverId only)
        var existingLocation = await context.DriverLocations
            .FirstOrDefaultAsync(l => l.DriverId == karimDriverId);

        if (existingLocation == null)
        {
            await context.DriverLocations.AddAsync(new DriverLocation
            {
                Id = Guid.NewGuid(),
                DriverId = karimDriverId,
                ActiveOrderId = testOrderId,
                Lat = 30.0500,
                Lng = 31.2400,
                RecordedAt = DateTime.UtcNow
            });
        }
        else
        {
            existingLocation.ActiveOrderId = testOrderId;
            existingLocation.Lat = 30.0500;
            existingLocation.Lng = 31.2400;
            existingLocation.RecordedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }
}
