using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.GetStores.Queries
{
    public sealed class GetStoresQueryValidator : AbstractValidator<GetStoresQuery>
    {
        public GetStoresQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("PageSize must be between 1 and 50.");
        }
    }
}
