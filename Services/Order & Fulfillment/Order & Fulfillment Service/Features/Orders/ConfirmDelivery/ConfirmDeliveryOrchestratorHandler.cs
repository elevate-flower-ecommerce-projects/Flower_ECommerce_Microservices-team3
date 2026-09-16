using Blocks.Contracts.Common;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.ConfirmDelivery.Commands;

namespace Order___Fulfillment_Service.Features.Orders.ConfirmDelivery
{

    public class ConfirmDeliveryOrchestratorHandler(IMediator mediator)
        : IRequestHandler<ConfirmDeliveryOrchestrator, Result<string>>
    {
        public async Task<Result<string>> Handle(
            ConfirmDeliveryOrchestrator request,
            CancellationToken cancellationToken)
        {
            
            var confirmResult = await mediator.Send(
                new ConfirmOrderDeliveryCommand(request.OrderId, request.CustomerId),
                cancellationToken);
            if (confirmResult.IsFailure)
            {
                return Result.Failure<string>(confirmResult.Error!);
            }
            var driverId = confirmResult.Value;
            
            if (driverId.HasValue)
            {
                await mediator.Send(
                    new ClearDriverActiveOrderCommand(driverId.Value),
                    cancellationToken);
            }
            return Result.Success("Order delivery confirmed successfully.");
        }
    }
}
