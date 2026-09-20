using Blocks.Domain.Entities;

namespace Address___Store_Coverage_Service.Entities
{
    public class City : AuditEntity
    {
        public Guid AreaId { get; set; }
        public Area Area { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
    }
}
