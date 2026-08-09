using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Contracts.Pagination;
using Identity.Application.Features.DriverApplicationReview.ViewModels;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Queries.QueryHandlers
{
    public class GetDriverApplicationsQueryHandler(IGenericRepository<Identity.Domain.Entities.DriverApplication> driverAppRepository)
    : IRequestHandler<GetDriverApplicationsQuery, Result<PagedResult<DriverApplicationSummaryVm>>>
    {
        public async Task<Result<PagedResult<DriverApplicationSummaryVm>>> Handle(GetDriverApplicationsQuery request,CancellationToken cancellationToken)
        {
           
            var query = driverAppRepository.GetQueryable()
                .Include(x => x.User)
                .AsNoTracking();

            if (request.Request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Request.Status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.Request.Pagination.PageNumber - 1) * request.Request.Pagination.PageSize)
                .Take(request.Request.Pagination.PageSize)
                .Select(x => new DriverApplicationSummaryVm(
                    x.Id,
                    x.UserId.Value,
                    $"{x.User.FirstName} {x.User.LastName}",
                    x.Status.ToString(),
                    x.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            var pagedResult = PagedResult<DriverApplicationSummaryVm>.Create(
                items,
                totalCount,
                request.Request.Pagination);

            return Result.Success(pagedResult);
        }
    }
}
