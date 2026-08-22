using Blocks.Contracts.Common;
using Identity.Application.Features.DriverApplicationReview.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Queries
{
    public record GetDriverApplicationByIdQuery(Guid Id): IRequest<Result<DriverApplicationDetailsVm>>;
}
