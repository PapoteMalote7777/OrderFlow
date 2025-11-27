using FluentValidation;
using TiendaGod.Identity.Controllers;

namespace TiendaGod.Identity.Validators
{
    public class UpdateUsernameModelValidator : AbstractValidator<UpdateUsernameModel>
    {
        public UpdateUsernameModelValidator()
        {
            RuleFor(x => x.NewName)
                .NotEmpty().WithMessage("El nombre no puede estar vacío");
        }
    }
}
