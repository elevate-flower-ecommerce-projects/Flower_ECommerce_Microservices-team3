using Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.Commands;
using FluentValidation;

namespace Address___Store_Coverage_Service.Features.Addresses.UpdateAddress.Validators
{
    public sealed class UpdateAddressCommandValidator
        : AbstractValidator<UpdateAddressCommand>
    {
        public UpdateAddressCommandValidator()
        {
            RuleFor(x => x.StoreId)
                .NotEmpty().WithMessage("Store ID is required.");
        }
    }
}
