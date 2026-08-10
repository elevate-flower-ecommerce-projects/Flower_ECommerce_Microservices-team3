using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Identity.Application.Features.DriverApplicationReview.DTOs;
using Identity.Application.Features.DriverApplicationReview.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Queries
{
    public record GetDriverApplicationsQuery(GetDriverApplicationsRequest Request)
    : IRequest<Result<PagedResult<DriverApplicationSummaryVm>>>;
}
