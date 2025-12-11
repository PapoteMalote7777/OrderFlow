using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TiendaGod.Identity.Services;

namespace TiendaGod.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserAdminController : ControllerBase
    {
        private readonly UserAdminService _userAdminService;

        public UserAdminController(UserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        [HttpPut("update-user/{userName}")]
        public async Task<IActionResult> UpdateUserNameByAdmin(string userName, [FromBody] AdminUpdateUsernameModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _userAdminService.UpdateUserNameAsync(userName, model.NewName);
                return Ok(new { message = "Nombre actualizado con éxito ✅" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete-user/{userName}")]
        public async Task<IActionResult> DeleteUserByAdmin(string userName)
        {
            try
            {
                await _userAdminService.DeleteUserAsync(userName);
                return Ok(new { message = "Cuenta eliminada correctamente ✅" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("admin-email/{index}")]
        public async Task<IActionResult> GetAdminEmail(int index)
        {
            var emails = await _userAdminService.GetAdminEmailsAsync();
            if (emails == null || !emails.Any()) return NotFound();

            if (index < 0 || index >= emails.Count) return NotFound();

            return Ok(new { Email = emails[index] });
        }

    }
    public record AdminUpdateUsernameModel(string NewName);
}
