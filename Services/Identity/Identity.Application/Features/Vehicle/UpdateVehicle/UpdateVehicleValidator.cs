using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Identity.Application.Features.Vehicle.UpdateVehicle
{
    public sealed class UpdateVehicleValidator
        : AbstractValidator<UpdateVehicleCommand>
    {
        public UpdateVehicleValidator()
        {
            RuleFor(x => x.VehicleType)
                .IsInEnum()
                .WithMessage("Invalid vehicle type.");

            RuleFor(x => x.VehicleNumber)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
