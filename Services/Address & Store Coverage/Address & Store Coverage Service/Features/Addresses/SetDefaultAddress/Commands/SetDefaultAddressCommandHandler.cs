using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress.DTOs;
using Address___Store_Coverage_Service.Persistence;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.SetDefaultAddress.Commands
{
    public sealed class SetDefaultAddressCommandHandler(
        IGenericRepository<Address> addressRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<SetDefaultAddressCommand, Result<SetDefaultAddressResponseDto>>
    {
        public async Task<Result<SetDefaultAddressResponseDto>> Handle(
            SetDefaultAddressCommand request,
            CancellationToken cancellationToken)
        {
            return await unitOfWork.ExecuteAsync(async () =>
            {
                var address = await addressRepository.GetQueryable()
                    .AsNoTracking()
                    .Where(a => a.Id == request.AddressId
                             && a.CustomerId == request.CustomerId
                             && a.DeletedAt == null)
                    .Select(a => new SetDefaultAddressResponseDto(
                        a.Id,
                        a.RecipientName,
                        a.Phone,
                        a.AddressLine,
                        a.CityId,
                        a.AreaId,
                        a.Latitude,
                        a.Longitude,
                        a.Label,
                        a.IsDefault,
                        a.StoreId,
                        a.CreatedAt))
                    .FirstOrDefaultAsync(cancellationToken);

                if (address is null)
                {
                    return Result.Failure<SetDefaultAddressResponseDto>(
                        Error.NotFound("Address not found."));
                }

                if (!address.IsDefault)
                {
                    await addressRepository.GetQueryable()
                        .Where(a => a.CustomerId == request.CustomerId
                                 && a.IsDefault
                                 && a.DeletedAt == null
                                 && a.Id != request.AddressId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(a => a.IsDefault, false)
                            .SetProperty(a => a.UpdatedAt, DateTime.UtcNow)
                            .SetProperty(a => a.UpdatedBy, request.CustomerId.ToString()),
                            cancellationToken);

                    await addressRepository.GetQueryable()
                        .Where(a => a.Id == request.AddressId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(a => a.IsDefault, true)
                            .SetProperty(a => a.UpdatedAt, DateTime.UtcNow)
                            .SetProperty(a => a.UpdatedBy, request.CustomerId.ToString()),
                            cancellationToken);

                    return Result.Success(address with { IsDefault = true });
                }

                return Result.Success(address);
            }, cancellationToken);
        }
    }
}
