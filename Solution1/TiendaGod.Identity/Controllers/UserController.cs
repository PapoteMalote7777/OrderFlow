using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace TiendaGod.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // 🔹 ACTUALIZAR NOMBRE (usuario autenticado)
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewName))
                return BadRequest(new { message = "El nombre no puede estar vacío" });

            var user = await GetUserFromClaims();
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            if (user.UserName == model.NewName)
                return Ok(new { message = "El nombre ya es el mismo" });

            var existing = await _userManager.FindByNameAsync(model.NewName);
            if (existing != null && existing.Id != user.Id)
                return BadRequest(new { message = "El nombre ya está en uso" });

            user.UserName = model.NewName;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "No se pudo actualizar el nombre", errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Nombre actualizado con éxito ✅" });
        }

        // 🔹 ELIMINAR CUENTA (usuario autenticado)
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await GetUserFromClaims();
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "Error al eliminar la cuenta", errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Cuenta eliminada correctamente ✅" });
        }

        private async Task<IdentityUser?> GetUserFromClaims()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null) return user;
            }

            return null;
        }

        // =========================
        // Operaciones administrativas (moved from RolesController)
        // =========================

        // Editar username de cualquier usuario (solo Admin)
        [Authorize(Roles = "Admin")]
        [HttpPut("update-user/{userName}")]
        public async Task<IActionResult> UpdateUserNameByAdmin(string userName, [FromBody] AdminUpdateUsernameModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewName))
                return BadRequest(new { message = "El nombre no puede estar vacío" });

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            var existing = await _userManager.FindByNameAsync(model.NewName);
            if (existing != null && existing.Id != user.Id)
                return BadRequest(new { message = "El nombre ya está en uso" });

            user.UserName = model.NewName;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "No se pudo actualizar el nombre", errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Nombre actualizado con éxito ✅" });
        }

        // Borrar usuario por username (solo Admin)
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user/{userName}")]
        public async Task<IActionResult> DeleteUserByAdmin(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "Error al eliminar la cuenta", errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Cuenta eliminada correctamente ✅" });
        }
    }

    // Modelos localizados al controlador
    public record UpdateUsernameModel(string NewName);
    public record AdminUpdateUsernameModel(string NewName);
}
