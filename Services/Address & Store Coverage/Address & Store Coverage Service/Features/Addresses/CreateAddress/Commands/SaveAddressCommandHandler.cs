using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Addresses.CreateAddress.DTOs;
using Address___Store_Coverage_Service.Persistence;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Commands
{
    public sealed class SaveAddressCommandHandler(
        IGenericRepository<Address> addressRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<SaveAddressCommand, Result<CreateAddressResponseDto>>
    {
        public async Task<Result<CreateAddressResponseDto>> Handle(
             SaveAddressCommand request,
             CancellationToken cancellationToken)
        {
            
            var hasDefault = await addressRepository.GetQueryable()
                .AnyAsync(a => a.CustomerId == request.CustomerId
                            && a.DeletedAt == null
                            && a.IsDefault,
                    cancellationToken);
           
            var totalEver = await addressRepository.GetQueryable()
                .CountAsync(a => a.CustomerId == request.CustomerId, cancellationToken);
            var label = string.IsNullOrWhiteSpace(request.Label)
                ? $"Address {totalEver + 1}"
                : request.Label.Trim();
           
            var address = new Address
            {
                CustomerId = request.CustomerId,
                RecipientName = request.RecipientName.Trim(),
                Phone = request.Phone.Trim(),
                AddressLine = request.AddressLine.Trim(),
                CityId = request.CityId,
                AreaId = request.AreaId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Label = label,
                IsDefault = !hasDefault,
                StoreId = request.StoreId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CustomerId.ToString(),
            };
            
            addressRepository.Add(address);
            await unitOfWork.SaveChangesAsync(cancellationToken);
           
            return Result.Success(new CreateAddressResponseDto(
                Id: address.Id,
                RecipientName: address.RecipientName,
                RecipientPhone: address.Phone,
                AddressLine: address.AddressLine,
                CityId: address.CityId,
                AreaId: address.AreaId,
                Lat: address.Latitude,
                Lng: address.Longitude,
                Label: address.Label,
                IsDefault: address.IsDefault,
                StoreId: address.StoreId,
                CreatedAt: address.CreatedAt));
        }
    }
}

