using System.Text.Json.Serialization;

namespace Address___Store_Coverage_Service.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CoverageBoundaryType
    {
        Polygon,
        Radius,
        CityAreaList
    }
}
