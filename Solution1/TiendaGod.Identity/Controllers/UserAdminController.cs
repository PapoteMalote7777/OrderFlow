using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TiendaGod.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserAdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-user/{userName}")]
        public async Task<IActionResult> UpdateUserNameByAdmin(string userName, [FromBody] AdminUpdateUsernameModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var existing = await _userManager.FindByNameAsync(model.NewName);
            if (existing != null && existing.Id != user.Id)
                return BadRequest(new { message = "El nombre ya está en uso" });

            user.UserName = model.NewName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Nombre actualizado con éxito ✅" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user/{userName}")]
        public async Task<IActionResult> DeleteUserByAdmin(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Cuenta eliminada correctamente ✅" });
        }
    }
    public record AdminUpdateUsernameModel(string NewName);
}
