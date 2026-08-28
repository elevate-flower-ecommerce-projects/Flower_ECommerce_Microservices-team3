using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.SetCoverageArea.Commands;
using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.SetCoverageArea.DTOs;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.SetCoverageArea
{
    public static class SetCoverageAreaEndpoint
    {
        public static IEndpointRouteBuilder MapSetCoverageAreaEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/admin/stores/{id:guid}/coverage-area", async (
                Guid id,
                CoverageAreaUpsertRequestDto request,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var command = new SetCoverageAreaCommand(
                    StoreId: id,
                    BoundaryType: request.BoundaryType,
                    Polygon: request.Polygon,
                    RadiusMeters: request.RadiusMeters,
                    Cities: request.Cities,
                    Areas: request.Areas);

                var result = await mediator.Send(command, ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<CoverageAreaDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<CoverageAreaDto>.Ok(result.Value, "Coverage area saved successfully."));
            })
            .WithName("AdminSetStoreCoverageArea")
            .WithTags("Admin - Store Coverage")
            .RequireAuthorization(FlowerClaimTypes.AdminPolicy);

            return app;
        }
    }
}
