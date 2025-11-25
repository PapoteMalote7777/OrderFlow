using FluentValidation;
using TiendaGod.Productos.DTO;

namespace TiendaGod.Productos.Validators
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MinimumLength(10);

            RuleFor(x => x.Price)
                .GreaterThan(0);

            RuleFor(x => x.Brand)
                .NotEmpty();
        }
    }
}
