using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace TiendaGod.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Solo Admin puede asignar roles
        [Authorize(Roles = "Admin")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest(new { message = "Rol inválido" });

            if (!await _userManager.IsInRoleAsync(user, model.Role))
            {
                var res = await _userManager.AddToRoleAsync(user, model.Role);
                if (!res.Succeeded) return BadRequest(res.Errors);
            }

            return Ok(new { message = "Rol asignado" });
        }

        // Solo Admin puede quitar roles
        [Authorize(Roles = "Admin")]
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest(new { message = "Rol inválido" });

            if (await _userManager.IsInRoleAsync(user, model.Role))
            {
                var res = await _userManager.RemoveFromRoleAsync(user, model.Role);
                if (!res.Succeeded) return BadRequest(res.Errors);
            }

            return Ok(new { message = "Rol removido" });
        }

        // Solo Admin puede listar usuarios con sus roles
        [Authorize(Roles = "Admin")]
        [HttpGet("list")]
        public async Task<IActionResult> ListUsers()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserWithRolesDto>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new UserWithRolesDto
                {
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            return Ok(result);
        }
    }

    public record AssignRoleModel(string UserName, string Role);

    public class UserWithRolesDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}