using FluentValidation;
using TiendaGod.Identity.Controllers;

namespace TiendaGod.Identity.Validators
{
    public class AdminUpdateUsernameModelValidator : AbstractValidator<AdminUpdateUsernameModel>
    {
        public AdminUpdateUsernameModelValidator()
        {
            RuleFor(x => x.NewName)
                .NotEmpty().WithMessage("El nombre no puede estar vacío");
        }
    }
}
