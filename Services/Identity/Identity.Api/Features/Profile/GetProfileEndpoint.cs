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
        app.MapGet("/api/users/GetProfile", async (HttpContext context, IMediator mediator) =>
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
        })
        .RequireAuthorization();
    }
}