using Identity.Application.Features.DriverApplicationReview.DTOs;
using Identity.Application.Features.DriverApplicationReview.Orchestrators;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Security.Claims;
using System.Threading;

namespace Identity.Api.Features.Admin
{
    public static class DriverApplicationReviewEndpoints
    {
        public static IEndpointRouteBuilder MapDriverApplicationReviewEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/admin/drivers/applications")
                           .WithTags("Admin Driver Applications")
                           .RequireAuthorization();

            group.MapPost("/{id:guid}/approve", async (
                Guid id,
                ClaimsPrincipal user,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var adminIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(adminIdString, out var adminId))
                    return Results.Unauthorized();

                var command = new ApproveDriverApplicationOrchestrator(id, adminId);
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Error);
            });



            group.MapPost("/{id:guid}/reject", async (
                Guid id,
                [FromBody] RejectApplicationRequest request,
                ClaimsPrincipal user,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var adminIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(adminIdString, out var adminId))
                    return Results.Unauthorized();

                var command = new RejectDriverApplicationOrchestrator(id, adminId, request.Reason);
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Error);
            });

            return app;
        }
    }
}