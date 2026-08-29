using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.GetStoreById.Queries
{
    public sealed class GetStoreByIdQueryValidator : AbstractValidator<GetStoreByIdQuery>
    {
        public GetStoreByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Store ID is required.");
        }
    }
}
