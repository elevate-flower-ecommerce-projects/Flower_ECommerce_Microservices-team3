using Blocks.Domain.Entities;

namespace Address___Store_Coverage_Service.Entities
{
    public class CoverageArea : AuditEntity
    {
        public Guid StoreId { get; set; }
        public Store Store { get; set; } = null!;

        public CoverageBoundaryType BoundaryType { get; set; }
        public double? RadiusMeters { get; set; }
        public List<GeoPoint>? Polygon { get; set; }
        public List<string>? Cities { get; set; }
        public List<string>? Areas { get; set; }
    }
}
