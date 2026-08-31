using Catalog_Service.Entities;
using Catalog_Service.GrpcServices;
using Catalog_Service.Persistence;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.GrpcServices
{
    public class StockGrpcService : StockService.StockServiceBase
    {
        private readonly FlowersCatalogDbContext _dbContext;

        public StockGrpcService(FlowersCatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<StockResponse> GetProductStock(StockRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.ProductId, out var productId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Product ID format."));
            }

            var product = await _dbContext.Products
                                          .IgnoreQueryFilters()
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return new StockResponse
                {
                    AvailableStock = 100
                };
            }

            int availableQuantity = (product.Status == ProductStatus.InStock) ? 100 : 0;
            return new StockResponse
            {
                AvailableStock = availableQuantity
            };
        }
    }
}