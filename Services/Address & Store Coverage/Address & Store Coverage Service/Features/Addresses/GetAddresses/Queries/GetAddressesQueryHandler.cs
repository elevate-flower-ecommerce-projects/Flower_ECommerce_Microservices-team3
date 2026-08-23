using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Addresses.GetAddresses.DTOs;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.GetAddresses.Queries
{
    public sealed class GetAddressesQueryHandler(
       IGenericRepository<Address> addressRepository)
       : IRequestHandler<GetAddressesQuery, Result<List<AddressItemDto>>>
    {
        public async Task<Result<List<AddressItemDto>>> Handle(
            GetAddressesQuery request,
            CancellationToken cancellationToken)
        {
            var addresses = await addressRepository.GetQueryable()
                .AsNoTracking()
                .Where(a => a.CustomerId == request.CustomerId
                         && a.DeletedAt == null)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .Select(a => new AddressItemDto(
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
                .ToListAsync(cancellationToken);
            return Result.Success(addresses);
        }
    }
}
