using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TiendaGod.Shared.Events;

namespace TiendaGod.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public UserController(UserManager<IdentityUser> userManager, IPublishEndpoint publishEndpoint)
        {
            _userManager = userManager;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await GetUserFromClaims();
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            if (user.UserName == model.NewName)
                return Ok(new { message = "El nombre ya es el mismo" });

            var existing = await _userManager.FindByNameAsync(model.NewName);
            if (existing != null && existing.Id != user.Id)
                return BadRequest(new { message = "El nombre ya está en uso" });

            user.UserName = model.NewName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            return Ok(new { message = "Nombre actualizado con éxito ✅" });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await GetUserFromClaims();
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            await _publishEndpoint.Publish(new UserDeletedEvent(
                user.Id,
                user.Email!
            ));

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

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

        [HttpGet("exists/{userId}")]
        public async Task<IActionResult> UserExists(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(new { exists = true });
        }

        [AllowAnonymous]
        [HttpGet("email/{userId}")]
        public async Task<IActionResult> GetUserEmail(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(new { Email = user.Email });
        }
    }
    public record UpdateUsernameModel(string NewName);
}
