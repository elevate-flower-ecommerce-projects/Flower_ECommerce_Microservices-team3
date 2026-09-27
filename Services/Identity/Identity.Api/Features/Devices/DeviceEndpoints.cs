using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Domain.Errors;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Api.Features.Devices;

public sealed record UpdateFcmTokenRequest(string DeviceId, string FcmToken);
public sealed record SetNotificationsRequest(string DeviceId, bool Enabled);

public static class DeviceEndpoints
{
    public static IEndpointRouteBuilder MapDeviceEndpoints(this IEndpointRouteBuilder app)
    {
        // ── 1. Update FCM Token ──
        var updateFcmHandler = async (
            UpdateFcmTokenRequest request,
            ClaimsPrincipal user,
            IDeviceRegistrationService deviceService,
            CancellationToken ct) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Json(
                    ApiResponse<object>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            if (string.IsNullOrWhiteSpace(request.DeviceId) || string.IsNullOrWhiteSpace(request.FcmToken))
            {
                return Results.Json(
                    ApiResponse<object>.Fail(Error.Validation("DeviceId and FcmToken are required.")),
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var success = await deviceService.UpdateFcmTokenAsync(
                userId,
                request.DeviceId.Trim(),
                request.FcmToken.Trim(),
                ct);

            return Results.Ok(ApiResponse<object>.Ok(
                new { deviceId = request.DeviceId, fcmToken = request.FcmToken },
                "FCM token updated successfully."));
        };

        app.MapPut("/api/v1/devices/fcm-token", updateFcmHandler)
            .WithName("UpdateFcmTokenV1")
            .WithTags("Devices")
            .WithSummary("Update Device FCM Token")
            .WithDescription("Updates the Firebase Cloud Messaging (FCM) push token for the authenticated user's device.")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        app.MapPost("/api/v1/devices/fcm-token", updateFcmHandler).ExcludeFromDescription().RequireAuthorization();
        app.MapPut("/api/devices/fcm-token", updateFcmHandler).ExcludeFromDescription().RequireAuthorization();
        app.MapPost("/api/devices/fcm-token", updateFcmHandler).ExcludeFromDescription().RequireAuthorization();
        app.MapPut("/devices/fcm-token", updateFcmHandler).ExcludeFromDescription().RequireAuthorization();

        // ── 2. Enable / Disable Notifications ──
        var notificationsHandler = async (
            SetNotificationsRequest request,
            ClaimsPrincipal user,
            IDeviceRegistrationService deviceService,
            CancellationToken ct) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Json(
                    ApiResponse<object>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            if (string.IsNullOrWhiteSpace(request.DeviceId))
            {
                return Results.Json(
                    ApiResponse<object>.Fail(Error.Validation("DeviceId is required.")),
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var success = await deviceService.SetNotificationsEnabledAsync(
                userId,
                request.DeviceId.Trim(),
                request.Enabled,
                ct);

            return Results.Ok(ApiResponse<object>.Ok(
                new { deviceId = request.DeviceId, notificationsEnabled = request.Enabled },
                request.Enabled ? "Notifications enabled successfully." : "Notifications disabled successfully."));
        };

        app.MapPut("/api/v1/devices/notifications", notificationsHandler)
            .WithName("SetDeviceNotificationsV1")
            .WithTags("Devices")
            .WithSummary("Enable / Disable Device Notifications")
            .WithDescription("Toggles push notifications on or off for the authenticated user's device.")
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        app.MapPatch("/api/v1/devices/notifications", notificationsHandler).ExcludeFromDescription().RequireAuthorization();
        app.MapPut("/api/devices/notifications", notificationsHandler).ExcludeFromDescription().RequireAuthorization();
        app.MapPatch("/api/devices/notifications", notificationsHandler).ExcludeFromDescription().RequireAuthorization();
        app.MapPut("/devices/notifications", notificationsHandler).ExcludeFromDescription().RequireAuthorization();

        return app;
    }
}
