namespace Catalog_Service.Features.Home.GetSections;

public sealed record HomeSectionResponse(
    Guid Id,
    string Type,
    int Index,
    bool IsActive,
    string Title,
    Guid? OccasionId,
    Guid? CategoryId);
