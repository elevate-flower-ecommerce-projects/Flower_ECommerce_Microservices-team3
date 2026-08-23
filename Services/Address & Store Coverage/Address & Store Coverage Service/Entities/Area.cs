using Blocks.Domain.Entities;

namespace Address___Store_Coverage_Service.Entities
{
    public class Area : AuditEntity
    {
        public Guid CityId { get; set; }
        public City City { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
    }
}
