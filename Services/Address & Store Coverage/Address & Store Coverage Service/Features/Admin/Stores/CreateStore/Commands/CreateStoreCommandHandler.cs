using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Persistence;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CreateStore.Commands
{
    public sealed class CreateStoreCommandHandler(
        IGenericRepository<Store> storeRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<CreateStoreCommand, Result<StoreDto>>
    {
        public async Task<Result<StoreDto>> Handle(
            CreateStoreCommand request,
            CancellationToken cancellationToken)
        {
            return await unitOfWork.ExecuteAsync(async () =>
            {
                var store = new Store
                {
                    Name = request.Name.Trim(),
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    CoverageRadiusKm = 10,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                storeRepository.Add(store);

                return await Task.FromResult(Result.Success(new StoreDto(
                    store.Id,
                    store.Name,
                    new GeoLocationDto(store.Latitude, store.Longitude),
                    store.IsActive,
                    store.CreatedAt)));
            }, cancellationToken);
        }
    }
}
