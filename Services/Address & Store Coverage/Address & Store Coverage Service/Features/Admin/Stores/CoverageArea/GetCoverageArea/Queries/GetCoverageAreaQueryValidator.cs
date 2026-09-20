using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.GetCoverageArea.Queries
{
    public sealed class GetCoverageAreaQueryValidator : AbstractValidator<GetCoverageAreaQuery>
    {
        public GetCoverageAreaQueryValidator()
        {
            RuleFor(x => x.StoreId)
                .NotEmpty()
                .WithMessage("Store ID is required.");
        }
    }
}
