using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.Features.DriverApplicationReview.Queries;
using Identity.Application.Features.DriverApplicationReview.ViewModels;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Application.Features.DriverApplicationReview.Queries.QueryHandlers
{
    public class GetDriverApplicationByIdQueryHandler(
        IGenericRepository<Identity.Domain.Entities.DriverApplication> driverAppRepository)
        : IRequestHandler<GetDriverApplicationByIdQuery, Result<DriverApplicationDetailsVm>>
    {
        public async Task<Result<DriverApplicationDetailsVm>> Handle(GetDriverApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var vm = await driverAppRepository.GetQueryable()
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Select(x => new DriverApplicationDetailsVm(
                    x.Id,
                    x.UserId ?? Guid.Empty,
                    x.User != null ? $"{x.User.FirstName} {x.User.LastName}" : "N/A",
                    x.User != null ? x.User.Email : "N/A",
                    x.User != null ? x.User.Phone : "N/A",
                    x.VehicleType.ToString(),
                    x.VehicleNumber,
                    x.VehicleLicenceImage,
                    x.NationalIdNumber,
                    x.NationalIdImage,
                    x.Status.ToString(),
                    x.RejectionReason,
                    x.CreatedAt,
                    x.ReviewedBy,
                    x.ReviewedAt
                ))
                .FirstOrDefaultAsync(cancellationToken);

            return vm is null
                ? Result.Failure<DriverApplicationDetailsVm>(Error.NotFound("Application not found."))
                : Result.Success(vm);
        }
    }
}