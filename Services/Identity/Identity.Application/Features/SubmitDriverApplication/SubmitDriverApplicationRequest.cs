using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Identity.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Features.Drivers.Commands.SubmitDriverApplication
{
    public sealed record SubmitDriverApplicationRequest(
        string CountryCode,
        string FirstName,
        string SecondName,
        VehicleType VehicleType,
        string VehicleNumber,
        string Email,
        string PhoneNumber,
        string NationalId,
        string Password,
        string ConfirmPassword,
        Gender Gender,
        IFormFile? VehicleLicenceFile,
        IFormFile? IdImage
    );
}
