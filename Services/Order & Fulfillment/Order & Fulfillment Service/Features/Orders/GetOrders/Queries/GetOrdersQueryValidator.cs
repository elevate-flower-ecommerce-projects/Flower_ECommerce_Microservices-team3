using FluentValidation;

namespace Order___Fulfillment_Service.Features.Orders.GetOrders.Queries
{
    public sealed class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
    {
        public GetOrdersQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be at least 1.");
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("PageSize must be between 1 and 50.");
        }
    }
}
