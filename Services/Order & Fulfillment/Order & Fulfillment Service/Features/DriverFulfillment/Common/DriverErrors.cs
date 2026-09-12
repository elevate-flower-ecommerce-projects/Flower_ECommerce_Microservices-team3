using Blocks.Domain.Errors;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.Common;

public static class DriverErrors
{
    public static Error OrderNotFound()
        => Error.NotFound("Order not found.");

    public static Error AlreadyHasActiveDelivery()
        => Error.Conflict("You already have an active delivery in progress.");

    public static Error OrderAlreadyClaimed()
        => Error.Conflict("This order has already been claimed by another driver.");

    public static Error OrderNotAssignedToYou()
        => Error.Forbidden("This order is not assigned to you.");

    public static Error InvalidStatusTransition(string currentStatus, string requestedStatus)
        => Error.Conflict($"Cannot transition from '{currentStatus}' to '{requestedStatus}'.");

    public static Error DriverUnauthorized()
        => Error.Unauthorized("Driver is not authenticated.");
}
