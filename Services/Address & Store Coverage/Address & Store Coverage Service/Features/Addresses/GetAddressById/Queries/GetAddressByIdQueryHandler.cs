using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Addresses.GetAddressById.DTOs;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.GetAddressById.Queries
{
    public sealed class GetAddressByIdQueryHandler(
       IGenericRepository<Address> addressRepository)
       : IRequestHandler<GetAddressByIdQuery, Result<AddressDetailsDto>>
    {
        public async Task<Result<AddressDetailsDto>> Handle(
            GetAddressByIdQuery request,
            CancellationToken cancellationToken)
        {
            var address = await addressRepository.GetQueryable()
                .AsNoTracking()
                .Where(a => a.Id == request.AddressId
                         && a.CustomerId == request.CustomerId
                         && a.DeletedAt == null)
                .Select(a => new AddressDetailsDto(
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
                return Result.Failure<AddressDetailsDto>(
                    Error.NotFound("Address not found."));
            return Result.Success(address);
        }
    }
}
