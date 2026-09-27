using Blocks.Contracts.Common;
using MediatR;

namespace Identity.Application.Features.Countries.GetCountries;

public sealed record GetCountriesQuery : IRequest<Result<List<CountryResponse>>>;
