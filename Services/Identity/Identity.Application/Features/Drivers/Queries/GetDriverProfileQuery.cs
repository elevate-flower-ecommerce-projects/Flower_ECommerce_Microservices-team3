using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Features.Drivers.Queries;

public sealed record GetDriverProfileQuery(Guid DriverId) : IRequest<DriverProfileResponse?>;
