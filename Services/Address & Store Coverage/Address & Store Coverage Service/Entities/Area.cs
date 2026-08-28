using Blocks.Domain.Entities;

namespace Address___Store_Coverage_Service.Entities
{
    public class Area : AuditEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
