using Blocks.Contracts.Common;
using Identity.Application.Features.Vehicle.VehicleTypes.GetVehicleTypes;
using Identity.Domain.Enums;
using MediatR;

namespace Identity.Application.Features.VehicleTypes.GetVehicleTypes;

public sealed record GetVehicleTypesQuery
    : IRequest<Result<List<VehicleTypeResponse>>>;

