using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.ViewModels
{
    public record DriverApplicationDetailsVm(
    Guid Id,
    Guid UserId,
    string FullName,
    string Email,
    string Phone,
    string VehicleType,
    string VehicleNumber,
    string VehicleLicenceImage,
    string NationalIdNumber,
    string NationalIdImage,
    string Status,
    string? RejectionReason,
    DateTime SubmittedAt,
    Guid? ReviewedBy,
    DateTime? ReviewedAt
    );
}
