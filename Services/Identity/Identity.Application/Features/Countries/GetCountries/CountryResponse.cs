using System.Text.Json.Serialization;

namespace Identity.Application.Features.Countries.GetCountries;

public sealed record CountryResponse(
    [property: JsonPropertyName("isoCode")] string IsoCode,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("phoneCode")] string PhoneCode,
    [property: JsonPropertyName("flag")] string Flag,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("latitude")] string Latitude,
    [property: JsonPropertyName("longitude")] string Longitude,
    [property: JsonPropertyName("timezones")] IReadOnlyList<CountryTimezoneResponse> Timezones
);

public sealed record CountryTimezoneResponse(
    [property: JsonPropertyName("zoneName")] string ZoneName,
    [property: JsonPropertyName("gmtOffset")] int GmtOffset,
    [property: JsonPropertyName("gmtOffsetName")] string GmtOffsetName,
    [property: JsonPropertyName("abbreviation")] string Abbreviation,
    [property: JsonPropertyName("tzName")] string TzName
);
