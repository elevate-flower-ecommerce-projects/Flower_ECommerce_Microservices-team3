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
        var handler = async (
            HttpContext context,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            string? fullName = null;
            string? email = null;
            string? phone = null;
            Gender? gender = null;
            IFormFile? photo = null;

            if (context.Request.HasFormContentType)
            {
                var form = await context.Request.ReadFormAsync(cancellationToken);
                fullName = form["fullName"].FirstOrDefault() ?? form["FullName"].FirstOrDefault();
                email = form["email"].FirstOrDefault() ?? form["Email"].FirstOrDefault();
                phone = form["phone"].FirstOrDefault() ?? form["Phone"].FirstOrDefault();

                var genderVal = form["gender"].FirstOrDefault() ?? form["Gender"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(genderVal))
                {
                    if (int.TryParse(genderVal, out var gInt) && Enum.IsDefined(typeof(Gender), gInt))
                    {
                        gender = (Gender)gInt;
                    }
                    else if (Enum.TryParse<Gender>(genderVal, true, out var gParsed))
                    {
                        gender = gParsed;
                    }
                }

                photo = form.Files.GetFile("photo")
                     ?? form.Files.GetFile("Photo")
                     ?? (form.Files.Count > 0 ? form.Files[0] : null);
            }
            else if (context.Request.HasJsonContentType())
            {
                var jsonBody = await context.Request.ReadFromJsonAsync<UpdateProfileRequest>(cancellationToken: cancellationToken);
                if (jsonBody is not null)
                {
                    fullName = jsonBody.FullName;
                    email = jsonBody.Email;
                    phone = jsonBody.Phone;
                    gender = jsonBody.Gender;
                }
            }

            var command = new UpdateProfileCommand(
                userId,
                fullName,
                email,
                phone,
                gender,
                photo
            );

            var result = await mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        };

        app.MapPut("/api/users/UpdateProfile", handler)
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("UpdateProfile")
            .WithTags("User Profile")
            .WithSummary("Update User Profile")
            .WithDescription("Updates user profile details (FullName, Email, Phone, Gender, Photo) using multipart/form-data or JSON.")
            .Produces<Identity.Application.Features.Profile.DTOs.ProfileResponseDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        app.MapPut("/api/users/profile", handler).ExcludeFromDescription().RequireAuthorization().DisableAntiforgery();
        app.MapPut("/api/v1/users/profile", handler).ExcludeFromDescription().RequireAuthorization().DisableAntiforgery();
    }
}