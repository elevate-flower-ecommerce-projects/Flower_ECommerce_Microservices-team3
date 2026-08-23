using Blocks.Domain.Entities;

namespace Address___Store_Coverage_Service.Entities
{
    public class Address : AuditEntity
    {
        public Guid CustomerId { get; set; }

        public string RecipientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;

        public Guid CityId { get; set; }
        public City City { get; set; } = null!;

        public Guid AreaId { get; set; }
        public Area Area { get; set; } = null!;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string? Label { get; set; }
        public bool IsDefault { get; set; }

        public Guid StoreId { get; set; }
    }
}
