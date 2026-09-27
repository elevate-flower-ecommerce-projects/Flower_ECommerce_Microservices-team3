using Blocks.Contracts.Interfaces;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Identity.Application.Interfaces;

namespace Identity.Application.Features.Drivers.Queries;

public sealed class GetDriverProfileQueryHandler(
    IGenericRepository<User> userRepository,
    IFileService fileService)
    : IRequestHandler<GetDriverProfileQuery, DriverProfileResponse?>
{
    public async Task<DriverProfileResponse?> Handle(
        GetDriverProfileQuery request,
        CancellationToken cancellationToken)
    {
        var driver = await userRepository.GetQueryable()
            .AsNoTracking()
            .Where(u => u.Id == request.DriverId && u.Role == UserRole.Driver && u.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (driver is null) return null;

        return new DriverProfileResponse(
            driver.Id,
            driver.FirstName,
            driver.LastName,
            driver.Phone,
            fileService.GetPublicUrl(driver.PhotoUrl));
    }
}
