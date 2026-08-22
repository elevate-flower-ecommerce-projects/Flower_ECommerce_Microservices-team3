using Blocks.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Orchestrators
{
    public record ApproveDriverApplicationOrchestrator(Guid ApplicationId, Guid AdminId) : IRequest<Result<bool>>;
}
