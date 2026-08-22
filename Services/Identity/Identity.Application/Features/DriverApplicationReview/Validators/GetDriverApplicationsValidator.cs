using FluentValidation;
using Identity.Application.Features.DriverApplicationReview.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Validators
{
    public class GetDriverApplicationsValidator : AbstractValidator<GetDriverApplicationsRequest>
    {
        public GetDriverApplicationsValidator()
        {
            RuleFor(x => x.Pagination)
                .NotNull().WithMessage("Pagination parameters are required.");

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue).WithMessage("Invalid application status.");
        }
    }
}
