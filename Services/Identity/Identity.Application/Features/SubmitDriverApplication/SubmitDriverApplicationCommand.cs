using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Contracts.Common;
using Blocks.Contracts.Http;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Features.Drivers.Commands.SubmitDriverApplication
{
    public sealed record SubmitDriverApplicationCommand(
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
    IFormFile? IdImage)
            : IRequest<Result<SubmitDriverApplicationResponse>>;

}
