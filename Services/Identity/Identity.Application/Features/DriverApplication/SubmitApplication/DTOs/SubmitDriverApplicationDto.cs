using Identity.Domain.Enums;

namespace Identity.Application.Features.DriverApplication.SubmitApplication.DTOs;

public class SubmitDriverApplicationDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public string NationalIdNumber { get; set; } = string.Empty;
    public string VehicleLicenceImage { get; set; } = string.Empty;
    public string NationalIdImage { get; set; } = string.Empty;
}
