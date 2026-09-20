using Blocks.Domain.Entities;

namespace Order___Fulfillment_Service.Entities;

public class DriverLocation : BaseEntity
{
    public Guid DriverId { get; set; }
    public Guid? ActiveOrderId { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public DateTime RecordedAt { get; set; }
}
