using Cart_Service.Features.GetCart.Queries;
using FluentValidation;

namespace Cart_Service.Features.GetCart.Validators
{
    public class GetCartQueryValidator : AbstractValidator<GetCartQuery>
    {
        public GetCartQueryValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required.");
        }
    }
}
