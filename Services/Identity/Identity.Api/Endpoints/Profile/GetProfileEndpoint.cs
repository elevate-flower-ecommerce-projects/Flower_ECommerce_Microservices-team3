using Identity.Application.Features.Profile.GetProfile.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Identity.Api.Features.Profile;

public static class GetProfileEndpoint
{
    public static void MapGetProfileEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (HttpContext context, IMediator mediator) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var query = new GetProfileQuery(userId);
            var result = await mediator.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Error);
        };

        app.MapGet("/api/users/GetProfile", handler)
            .RequireAuthorization()
            .WithName("GetProfile")
            .WithTags("User Profile")
            .WithSummary("Get Current User Profile")
            .Produces<Identity.Application.Features.Profile.DTOs.ProfileResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/api/users/profile", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapGet("/api/v1/users/profile", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapGet("/api/users/me", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapGet("/users/me/profile", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapGet("/users/me", handler).ExcludeFromDescription().RequireAuthorization();
    }
}