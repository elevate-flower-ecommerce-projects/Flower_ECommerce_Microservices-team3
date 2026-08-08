using Blocks.Contracts.Pagination;
using Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.DTOs
{
    public record GetDriverApplicationsRequest(
    PaginationParams Pagination,
    DriverApplicationStatus? Status = null
    );
}
