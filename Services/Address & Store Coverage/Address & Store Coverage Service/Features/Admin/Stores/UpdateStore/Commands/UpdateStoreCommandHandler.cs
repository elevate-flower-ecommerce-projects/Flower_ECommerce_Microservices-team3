using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Persistence;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.UpdateStore.Commands
{
    public sealed class UpdateStoreCommandHandler(
        IGenericRepository<Store> storeRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateStoreCommand, Result<StoreDto>>
    {
        public async Task<Result<StoreDto>> Handle(
            UpdateStoreCommand request,
            CancellationToken cancellationToken)
        {
            return await unitOfWork.ExecuteAsync(async () =>
            {
                var store = await storeRepository.GetQueryable()
                    .FirstOrDefaultAsync(s => s.Id == request.Id && s.DeletedAt == null, cancellationToken);

                if (store is null)
                {
                    return Result.Failure<StoreDto>(Error.NotFound("Store not found."));
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    store.Name = request.Name.Trim();
                }

                if (request.Location is not null)
                {
                    store.Latitude = request.Location.Lat;
                    store.Longitude = request.Location.Lng;
                }

                if (request.IsActive.HasValue)
                {
                    store.IsActive = request.IsActive.Value;
                }

                store.UpdatedAt = DateTime.UtcNow;

                storeRepository.Update(store);

                return Result.Success(new StoreDto(
                    store.Id,
                    store.Name,
                    new GeoLocationDto(store.Latitude, store.Longitude),
                    store.IsActive,
                    store.CreatedAt));
            }, cancellationToken);
        }
    }
}
