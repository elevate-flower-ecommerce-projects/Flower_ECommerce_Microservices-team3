using Blocks.Domain.Entities;

namespace Address___Store_Coverage_Service.Entities
{
    public class City : AuditEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<Area> Areas { get; set; } = new List<Area>();
    }
}
