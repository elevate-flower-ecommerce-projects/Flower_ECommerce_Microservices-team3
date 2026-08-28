using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.GetCoverageArea.Queries;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.GetCoverageArea
{
    public static class GetCoverageAreaEndpoint
    {
        public static IEndpointRouteBuilder MapGetCoverageAreaEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/admin/stores/{id:guid}/coverage-area", async (
                Guid id,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetCoverageAreaQuery(id), ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<CoverageAreaDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<CoverageAreaDto>.Ok(result.Value));
            })
            .WithName("AdminGetStoreCoverageArea")
            .WithTags("Admin - Store Coverage")
            .RequireAuthorization(FlowerClaimTypes.AdminPolicy);

            return app;
        }
    }
}
