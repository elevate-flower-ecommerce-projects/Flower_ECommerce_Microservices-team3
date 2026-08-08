using Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.DTOs
{
    public record CreateDriverDto(
    Guid UserId,
    Guid DriverApplicationId,
    VehicleType VehicleType,
    string VehicleNumber,
    string VehicleLicenceImage,
    string NationalIdNumber,
    string NationalIdImage
    );
}
