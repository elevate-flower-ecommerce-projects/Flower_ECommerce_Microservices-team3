using Blocks.Contracts.Common;
using Blocks.Contracts.Events;
using MediatR;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Commands.CommandHandlers
{
    public class PublishDriverApprovedEventCommandHandler(IPublishEndpoint publishEndpoint)
        : IRequestHandler<PublishDriverApprovedEventCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(PublishDriverApprovedEventCommand request, CancellationToken cancellationToken)
        {
            await publishEndpoint.Publish(new DriverApplicationApprovedEvent(request.UserId, request.Email), cancellationToken);
            return Result.Success(true);
        }
    }
}
