namespace Address___Store_Coverage_Service.Features.Areas.DTOs
{
    public sealed record CityDto(Guid Id, string Name);

    public sealed record AreaWithCitiesDto(Guid Id, string Name, List<CityDto> Cities);
}
