using Blocks.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Commands
{
    public record RejectDriverApplicationCommand(Guid ApplicationId, Guid AdminId, string Reason) : IRequest<Result<bool>>;
}
