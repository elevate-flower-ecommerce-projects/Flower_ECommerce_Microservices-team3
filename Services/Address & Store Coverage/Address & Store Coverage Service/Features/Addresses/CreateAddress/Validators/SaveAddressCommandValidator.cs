using Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Commands;
using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Addresses.CreateAddress.Validators
{
    public sealed class SaveAddressCommandValidator : AbstractValidator<SaveAddressCommand>
    {
        public SaveAddressCommandValidator()
        {
            RuleFor(x => x.StoreId)
                .NotEmpty().WithMessage("Store ID is required.");
        }
    }
}
