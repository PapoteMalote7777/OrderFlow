using FluentValidation;
using TiendaGod.Identity.Controllers;

namespace TiendaGod.Identity.Validators
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio")
                .EmailAddress().WithMessage("Correo inválido");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")
                .WithMessage("La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número");
        }
    }
}
