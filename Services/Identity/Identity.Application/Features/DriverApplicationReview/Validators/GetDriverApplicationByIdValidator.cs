using FluentValidation;
using Identity.Application.Features.DriverApplicationReview.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Validators
{
    public class GetDriverApplicationByIdValidator : AbstractValidator<GetDriverApplicationByIdQuery>
    {
        public GetDriverApplicationByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Application ID is required.");
        }
    }
}
