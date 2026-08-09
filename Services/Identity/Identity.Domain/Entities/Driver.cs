using Blocks.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public class Driver : AuditEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public VehicleType VehicleType { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public string VehicleLicenceImage { get; set; } = string.Empty;

    public string? NationalIdNumber { get; set; } = string.Empty;
    public string? NationalIdImage { get; set; } = string.Empty;
}
