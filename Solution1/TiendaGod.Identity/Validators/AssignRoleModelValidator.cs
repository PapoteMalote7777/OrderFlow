using FluentValidation;
using TiendaGod.Identity.Controllers;

namespace TiendaGod.Identity.Validators
{
    public class AssignRoleModelValidator : AbstractValidator<AssignRoleModel>
    {
        public AssignRoleModelValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("El rol es obligatorio");
        }
    }
}
