using FluentValidation;
using Identity.Application.Features.DriverApplicationReview.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Validators
{
    public class RejectApplicationRequestValidator : AbstractValidator<RejectApplicationRequest>
    {
        public RejectApplicationRequestValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Rejection reason is required.")
                .MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters.");
        }
    }
}
