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

namespace Identity.Application.Features.DriverApplicationReview.Queries.QueryHandlers;

public class GetDriverApplicationByIdQueryHandler(
    IGenericRepository<DriverApplication> driverAppRepository)
    : IRequestHandler<GetDriverApplicationByIdQuery, Result<DriverApplicationDetailsVm>>
{
    public async Task<Result<DriverApplicationDetailsVm>> Handle(GetDriverApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var application = await driverAppRepository.GetQueryable()
            .Include(x => x.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (application is null)
        {
            return Result.Failure<DriverApplicationDetailsVm>(
                Error.NotFound("Application not found."));
        }

        var viewModel = new DriverApplicationDetailsVm(
            application.Id,
            application.UserId.Value,
            $"{application.User.FirstName} {application.User.LastName}",
            application.User.Email,
            application.User.Phone,
            application.VehicleType.ToString(),
            application.VehicleNumber,
            application.VehicleLicenceImage,
            application.NationalIdNumber,
            application.NationalIdImage,
            application.Status.ToString(),
            application.RejectionReason,
            application.CreatedAt,
            application.ReviewedBy,
            application.ReviewedAt
        );

        return Result.Success(viewModel);
    }
}