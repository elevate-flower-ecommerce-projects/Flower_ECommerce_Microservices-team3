using Blocks.Contracts.Common;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Features.Countries.GetCountries;

public sealed class GetCountriesHandler(ICountryDataProvider countryDataProvider)
    : IRequestHandler<GetCountriesQuery, Result<List<CountryResponse>>>
{
    public Task<Result<List<CountryResponse>>> Handle(
        GetCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var countries = countryDataProvider.GetCountries().ToList();
        return Task.FromResult(Result.Success(countries));
    }
}
