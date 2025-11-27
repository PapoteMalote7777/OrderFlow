using FluentValidation;
using TiendaGod.Identity.Controllers;

namespace TiendaGod.Identity.Validators
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria");
        }
    }
}
