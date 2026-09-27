using Blocks.Contracts.Http;
using Identity.Application.Features.Countries.GetCountries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Api.Features.Countries;

public static class GetCountriesEndpoint
{
    public static IEndpointRouteBuilder MapGetCountriesEndpoint(
        this IEndpointRouteBuilder app)
    {
        var handler = async (
            [FromQuery] bool? wrapped,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetCountriesQuery(),
                cancellationToken);

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result.Error);
            }

            if (wrapped == true)
            {
                return Results.Ok(ApiResponse<List<CountryResponse>>.Ok(result.Value));
            }

            return Results.Ok(result.Value);
        };

        app.MapGet("/api/v1/countries", handler)
            .WithName("GetCountries")
            .WithTags("Lookups")
            .WithSummary("Get Countries (ISO, Dial Codes, Flags, Currencies, Timezones)")
            .WithDescription("Returns supported countries matching client requirements: isoCode, name, phoneCode, flag, currency, latitude, longitude, and timezones. Pass ?wrapped=true for ApiResponse envelope.")
            .Produces<List<CountryResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<List<CountryResponse>>>(StatusCodes.Status200OK);

        app.MapGet("/api/countries", handler).ExcludeFromDescription();
        app.MapGet("/countries", handler).ExcludeFromDescription();

        return app;
    }
}
