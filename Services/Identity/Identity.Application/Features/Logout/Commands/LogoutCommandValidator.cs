using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Logout.Commands
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId could not be resolved from the access token.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("DeviceId is required.")
                .MaximumLength(128).WithMessage("DeviceId must not exceed 128 characters.");
        }
    }
}
