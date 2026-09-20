using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Persistence;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.DeleteStore.Commands
{
    public sealed class DeleteStoreCommandHandler(
        IGenericRepository<Store> storeRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteStoreCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            DeleteStoreCommand request,
            CancellationToken cancellationToken)
        {
            return await unitOfWork.ExecuteAsync(async () =>
            {
                var store = await storeRepository.GetQueryable()
                    .FirstOrDefaultAsync(s => s.Id == request.Id && s.DeletedAt == null, cancellationToken);

                if (store is null)
                {
                    return Result.Failure<string>(Error.NotFound("Store not found."));
                }

                store.Deactivate();
                store.UpdatedAt = DateTime.UtcNow;

                storeRepository.Update(store);

                return Result.Success("Store deactivated successfully.");
            }, cancellationToken);
        }
    }
}
