using Blocks.Contracts.Interfaces;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Features.Drivers.Queries;

public sealed class GetDriverProfileQueryHandler(
    IGenericRepository<User> userRepository)
    : IRequestHandler<GetDriverProfileQuery, DriverProfileResponse?>
{
    public async Task<DriverProfileResponse?> Handle(
        GetDriverProfileQuery request,
        CancellationToken cancellationToken)
    {
        return await userRepository.GetQueryable()
            .AsNoTracking()
            .Where(u => u.Id == request.DriverId && u.Role == UserRole.Driver && u.DeletedAt == null)
            .Select(u => new DriverProfileResponse(
                u.Id,
                u.FirstName,
                u.LastName,
                u.Phone,
                u.PhotoUrl))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
