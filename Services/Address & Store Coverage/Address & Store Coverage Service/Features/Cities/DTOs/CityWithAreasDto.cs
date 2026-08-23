namespace Address___Store_Coverage_Service.Features.Cities.DTOs
{
    public sealed record AreaDto(Guid Id, string Name);

    
    public sealed record CityWithAreasDto(Guid Id, string Name, List<AreaDto> Areas);
}
