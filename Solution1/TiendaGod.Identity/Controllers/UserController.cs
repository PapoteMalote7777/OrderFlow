using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

        // 🔹 ACTUALIZAR NOMBRE
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
                return BadRequest(new { message = "No se pudo actualizar el nombre" });

            return Ok(new { message = "Nombre actualizado con éxito ✅" });
        }

        // 🔹 ELIMINAR CUENTA
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await GetUserFromClaims();
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "Error al eliminar la cuenta" });

            return Ok(new { message = "Cuenta eliminada correctamente ✅" });
        }

        private async Task<IdentityUser?> GetUserFromClaims()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user != null) return user;
            }

            return null;
        }
    }

    public record UpdateUsernameModel(string NewName);
}
