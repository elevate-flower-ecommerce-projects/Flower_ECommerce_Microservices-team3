using Identity.Application.Features.Profile.UpdateProfile.Commands;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Identity.Api.Features.Profile;

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Gender? Gender { get; set; }
    public IFormFile? Photo { get; set; }
}

public static class UpdateProfileEndpoint
{
    public static void MapUpdateProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/UpdateProfile", async (
            [AsParameters] UpdateProfileRequest request,
            HttpContext context,
            IMediator mediator) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new UpdateProfileCommand(
                userId,
                request.FullName,
                request.Email,
                request.Phone,
                request.Gender,
                request.Photo
            );

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequireAuthorization()
        .DisableAntiforgery();
    }
}