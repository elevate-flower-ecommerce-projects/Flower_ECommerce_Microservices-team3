using Blocks.Contracts.Common;
using Blocks.Contracts.Events;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Commands.CommandHandlers
{
    public class PublishDriverRejectedEventCommandHandler(IPublishEndpoint publishEndpoint)
        : IRequestHandler<PublishDriverRejectedEventCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(PublishDriverRejectedEventCommand request, CancellationToken cancellationToken)
        {
            await publishEndpoint.Publish(new DriverApplicationRejectedEvent(request.Email, request.Reason), cancellationToken);
            return Result.Success(true);
        }
    }
}
