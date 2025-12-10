using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TiendaGod.Identity.Services;
using TiendaGod.Shared.Events;

namespace TiendaGod.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.GetUserFromClaimsAsync(User);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            try
            {
                await _userService.UpdateUsernameAsync(user, model.NewName);
                return Ok(new { message = "Nombre actualizado con éxito ✅" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userService.GetUserFromClaimsAsync(User);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            try
            {
                await _userService.DeleteAccountAsync(user);
                return Ok(new { message = "Cuenta eliminada correctamente ✅" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("exists/{userId}")]
        public async Task<IActionResult> UserExists(string userId)
        {
            var exists = await _userService.UserExistsAsync(userId);
            if (!exists) return NotFound(new { message = "Usuario no encontrado" });
            return Ok(new { exists = true });
        }

        [AllowAnonymous]
        [HttpGet("email/{userId}")]
        public async Task<IActionResult> GetUserEmail(string userId)
        {
            var email = await _userService.GetUserEmailAsync(userId);
            if (email == null) return NotFound();
            return Ok(new { Email = email });
        }
    }
    public record UpdateUsernameModel(string NewName);
}
