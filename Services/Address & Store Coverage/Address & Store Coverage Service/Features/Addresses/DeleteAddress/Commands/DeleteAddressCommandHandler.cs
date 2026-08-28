using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Persistence;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.DeleteAddress.Commands
{
    public sealed class DeleteAddressCommandHandler(
       IGenericRepository<Address> addressRepository,
       IUnitOfWork unitOfWork)
       : IRequestHandler<DeleteAddressCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            DeleteAddressCommand request,
            CancellationToken cancellationToken)
        {
            var address = await addressRepository.GetQueryable()
            .FirstOrDefaultAsync(a => a.Id == request.AddressId
                               && a.CustomerId == request.CustomerId
                               && a.DeletedAt == null,
                             cancellationToken);
            if (address is null)
            {
                return Result.Failure<string>(Error.NotFound("Address not found."));
            }
            var wasDefault = address.IsDefault;


            address.DeletedAt = DateTime.UtcNow;
            address.DeletedBy = request.CustomerId.ToString();


            if (wasDefault)
            {
                address.IsDefault = false;
                var nextDefaultAddress = await addressRepository.GetQueryable()
                    .Where(a => a.CustomerId == request.CustomerId
                             && a.Id != request.AddressId
                             && a.DeletedAt == null)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);
                if (nextDefaultAddress is not null)
                {
                    nextDefaultAddress.IsDefault = true;
                    nextDefaultAddress.UpdatedAt = DateTime.UtcNow;
                    nextDefaultAddress.UpdatedBy = request.CustomerId.ToString();
                }
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success("Address deleted successfully");
        }


    }
    }

