using Blocks.Contracts.Pagination;
using Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.DTOs
{
    public record GetDriverApplicationsRequest(
        int PageNumber = 1,
        int PageSize = 10,
        DriverApplicationStatus? Status = null
    )
    {
        public PaginationParams Pagination => new PaginationParams
        {
            PageNumber = PageNumber,
            PageSize = PageSize
        };
    }
}
