using FluentValidation;
using TiendaGod.Productos.Models;

namespace TiendaGod.Productos.Validators
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .WithMessage("El nombre de la categoría debe tener al menos 3 caracteres");
        }
    }
}
