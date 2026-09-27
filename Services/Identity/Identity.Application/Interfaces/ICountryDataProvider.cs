using Identity.Application.Features.Countries.GetCountries;

namespace Identity.Application.Interfaces;

public interface ICountryDataProvider
{
    IReadOnlyList<CountryResponse> GetCountries();
    Task SeedCountriesAsync(CancellationToken cancellationToken = default);
}
