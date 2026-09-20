namespace Address___Store_Coverage_Service.Features.NearestCoveringStore.DTOs
{
    public sealed record NearestStoreDto(
        Guid StoreId,
        double DistanceKm = 0.0,
        string StoreName = ""
    );
}
