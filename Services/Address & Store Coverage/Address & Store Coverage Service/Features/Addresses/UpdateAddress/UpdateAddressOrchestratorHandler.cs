using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Queries;
using Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.Commands;
using Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.DTOs;
using Address___Store_Coverage_Service.Features.NearestCoveringStore.Queries;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.UpdateAddress
{
    public sealed class UpdateAddressOrchestratorHandler(
        IGenericRepository<Address> addressRepository,
        IMediator mediator)
        : IRequestHandler<UpdateAddressOrchestrator, Result<UpdateAddressResponseDto>>
    {
        public async Task<Result<UpdateAddressResponseDto>> Handle(
           UpdateAddressOrchestrator request,
           CancellationToken cancellationToken)
        {
            var exists = await addressRepository.GetQueryable()
                .AsNoTracking()
                .AnyAsync(a => a.Id == request.AddressId
                            && a.CustomerId == request.CustomerId
                            && a.DeletedAt == null,
                          cancellationToken);

            if (!exists)
            {
                return Result.Failure<UpdateAddressResponseDto>(Error.NotFound("Address not found."));
            }

            var cityAreaResult = await mediator.Send(
                new ValidateCityAreaQuery(request.CityId, request.AreaId),
                cancellationToken);

            if (cityAreaResult.IsFailure)
            {
                return Result.Failure<UpdateAddressResponseDto>(cityAreaResult.Error!);
            }

            var coverageResult = await mediator.Send(
                new FindNearestCoveringStoreQuery(request.Latitude, request.Longitude),
                cancellationToken);

            if (coverageResult.IsFailure)
            {
                return Result.Failure<UpdateAddressResponseDto>(coverageResult.Error!);
            }

            var updateResult = await mediator.Send(
                new UpdateAddressCommand(
                    AddressId: request.AddressId,
                    CustomerId: request.CustomerId,
                    RecipientName: request.RecipientName,
                    Phone: request.Phone,
                    AddressLine: request.AddressLine,
                    CityId: request.CityId,
                    AreaId: request.AreaId,
                    Latitude: request.Latitude,
                    Longitude: request.Longitude,
                    Label: request.Label,
                    StoreId: coverageResult.Value.StoreId),
                cancellationToken);

            return updateResult;
        }
    }
}
