using Address___Store_Coverage_Service.Features.Admin.Stores.DeleteStore.Commands;
using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.DeleteStore.Validators
{
    public sealed class DeleteStoreCommandValidator : AbstractValidator<DeleteStoreCommand>
    {
        public DeleteStoreCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Store ID is required.");
        }
    }
}
