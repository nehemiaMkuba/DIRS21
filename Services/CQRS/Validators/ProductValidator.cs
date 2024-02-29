using FluentValidation;
using CQRS.Commands;

namespace CQRS.Validators
{
    public class ProductValidator : AbstractValidator<AddProductCommand>
    {
        public ProductValidator()
        {
            RuleFor(command => command.Product.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(3, 20).WithMessage("Name must be between 3 and 20 characters.");

        }
    }
}
