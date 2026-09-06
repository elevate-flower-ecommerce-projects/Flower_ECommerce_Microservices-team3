using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.Checkout.EstimateDelivery;

public sealed class EstimateDeliveryQueryHandler : IRequestHandler<EstimateDeliveryQuery, Result<EstimateDeliveryResponse>>
{
    private readonly IAddressServiceClient _addressService;

    public EstimateDeliveryQueryHandler(IAddressServiceClient addressService)
    {
        _addressService = addressService;
    }

    public async Task<Result<EstimateDeliveryResponse>> Handle(
        EstimateDeliveryQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Fetch address details
        var address = await _addressService.GetAddressByIdAsync(
            request.AddressId,
            request.BearerToken,
            cancellationToken);

        if (address is null)
        {
            return Result.Failure<EstimateDeliveryResponse>(
                Error.NotFound("Address not found."));
        }

        // 2. Check store coverage
        var coverage = await _addressService.GetNearestCoveringStoreAsync(
            address.Latitude,
            address.Longitude,
            cancellationToken);

        if (coverage is null || !coverage.IsServiceable)
        {
            return Result.Failure<EstimateDeliveryResponse>(
                Error.Validation("Delivery is not available to this address. Please choose another address.", "addressId"));
        }

        // 3. Compute estimated delivery timestamp
        var estimatedDeliveryAt = DateTime.UtcNow.AddMinutes(coverage.EstimatedDeliveryMinutes).ToString("g");

        return Result.Success(new EstimateDeliveryResponse(estimatedDeliveryAt));
    }
}
