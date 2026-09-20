using Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Commands;
using Address___Store_Coverage_Service.Features.Addresses.CreateAddress.DTOs;
using Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Queries;
using Address___Store_Coverage_Service.Features.NearestCoveringStore.Queries;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress
{
    public sealed class CreateAddressOrchestratorHandler(
        IMediator mediator)
        : IRequestHandler<CreateAddressOrchestrator, Result<CreateAddressResponseDto>>
    {
        public async Task<Result<CreateAddressResponseDto>> Handle(
           CreateAddressOrchestrator request,
           CancellationToken cancellationToken)
        {
           
            var cityAreaResult = await mediator.Send(
                new ValidateCityAreaQuery(request.CityId, request.AreaId),
                cancellationToken);
            if (cityAreaResult.IsFailure)
                return Result.Failure<CreateAddressResponseDto>(cityAreaResult.Error!);
            
            var coverageResult = await mediator.Send(
                new FindNearestCoveringStoreQuery(request.Latitude, request.Longitude),
                cancellationToken);
            if (coverageResult.IsFailure)
                return Result.Failure<CreateAddressResponseDto>(coverageResult.Error!);
           
            var saveResult = await mediator.Send(
                new SaveAddressCommand(
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
            return saveResult;
        }
    }
}
