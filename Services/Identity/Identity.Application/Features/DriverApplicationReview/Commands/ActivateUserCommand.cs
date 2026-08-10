using Blocks.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Commands
{
    public record ActivateUserCommand(Guid UserId) : IRequest<Result<bool>>;
}
