using FluentValidation;
using TiendaGod.Productos.Models.DTO;

namespace TiendaGod.Productos.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
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
