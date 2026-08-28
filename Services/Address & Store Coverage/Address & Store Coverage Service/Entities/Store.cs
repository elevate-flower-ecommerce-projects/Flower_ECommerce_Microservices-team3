using Blocks.Domain.Entities;

namespace Address___Store_Coverage_Service.Entities
{
    public class Store : AuditEntity
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double CoverageRadiusKm { get; set; }

        public CoverageArea? CoverageArea { get; set; }
    }
}
