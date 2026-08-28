using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Addresses.Common;
using Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.DTOs;
using Address___Store_Coverage_Service.Persistence;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.Commands
{
    public sealed class UpdateAddressCommandHandler(
        IGenericRepository<Address> addressRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateAddressCommand, Result<UpdateAddressResponseDto>>
    {
        public async Task<Result<UpdateAddressResponseDto>> Handle(
             UpdateAddressCommand request,
             CancellationToken cancellationToken)
        {
            var address = await addressRepository.GetQueryable()
                .FirstOrDefaultAsync(a => a.Id == request.AddressId
                                       && a.CustomerId == request.CustomerId
                                       && a.DeletedAt == null,
                                     cancellationToken);
            if (address is null)
            {
                return Result.Failure<UpdateAddressResponseDto>(
                    Error.NotFound("Address not found."));
            }

            var label = string.IsNullOrWhiteSpace(request.Label)
               ? await AddressLabelGenerator.GenerateAsync(
                     addressRepository.GetQueryable(), request.CustomerId, address.Id, cancellationToken)
               : request.Label.Trim();


            address.RecipientName = request.RecipientName.Trim();
            address.Phone = request.Phone.Trim();
            address.AddressLine = request.AddressLine.Trim();
            address.CityId = request.CityId;
            address.AreaId = request.AreaId;
            address.Latitude = request.Latitude;
            address.Longitude = request.Longitude;
            address.Label = label;
            address.StoreId = request.StoreId;
            address.UpdatedAt = DateTime.UtcNow;
            address.UpdatedBy = request.CustomerId.ToString();


            await unitOfWork.SaveChangesAsync(cancellationToken);


            return Result.Success(new UpdateAddressResponseDto(
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
