using System.Reflection;
using System.Text.Json;
using Identity.Application.Features.Countries.GetCountries;
using Identity.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Services;

public sealed class CountryDataProvider : ICountryDataProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly object SyncLock = new();
    private static IReadOnlyList<CountryResponse>? _cachedCountries;
    private readonly ILogger<CountryDataProvider> _logger;

    public CountryDataProvider(ILogger<CountryDataProvider> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<CountryResponse> GetCountries()
    {
        if (_cachedCountries != null && _cachedCountries.Count > 0)
        {
            return _cachedCountries;
        }

        lock (SyncLock)
        {
            if (_cachedCountries != null && _cachedCountries.Count > 0)
            {
                return _cachedCountries;
            }

            _cachedCountries = LoadCountries();
            return _cachedCountries;
        }
    }

    public Task SeedCountriesAsync(CancellationToken cancellationToken = default)
    {
        var list = GetCountries();
        _logger.LogInformation("CountrySeeder: Successfully seeded {Count} countries for Identity and driver applications.", list.Count);
        return Task.CompletedTask;
    }

    private IReadOnlyList<CountryResponse> LoadCountries()
    {
        try
        {
            // 1. Try file from output directory
            var candidatePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Persistence", "Data", "countries.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "countries.json"),
                Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "countries.json"),
                Path.Combine(AppContext.BaseDirectory, "countries.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "Persistence", "Data", "countries.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "Services", "Identity", "Identity.Infrastructure", "Persistence", "Data", "countries.json")
            };

            foreach (var path in candidatePaths)
            {
                if (File.Exists(path))
                {
                    _logger.LogInformation("Loading countries from file: {Path}", path);
                    var json = File.ReadAllText(path);
                    var parsed = JsonSerializer.Deserialize<List<CountryResponse>>(json, JsonOptions);
                    if (parsed != null && parsed.Count > 0)
                    {
                        return parsed;
                    }
                }
            }

            // 2. Try embedded resource
            var assembly = typeof(CountryDataProvider).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();
            var countryResourceName = resourceNames.FirstOrDefault(r => r.EndsWith("countries.json", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(countryResourceName))
            {
                _logger.LogInformation("Loading countries from embedded resource: {Resource}", countryResourceName);
                using var stream = assembly.GetManifestResourceStream(countryResourceName);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    var json = reader.ReadToEnd();
                    var parsed = JsonSerializer.Deserialize<List<CountryResponse>>(json, JsonOptions);
                    if (parsed != null && parsed.Count > 0)
                    {
                        return parsed;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load countries from JSON seed file or embedded resource.");
        }

        _logger.LogWarning("No countries could be loaded from countries.json; returning empty list.");
        return Array.Empty<CountryResponse>();
    }
}
