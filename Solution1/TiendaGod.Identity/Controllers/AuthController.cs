using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace TiendaGod.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // 🔹 REGISTRO
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validaciones adicionales
            if (!ValidatePassword(model.Password))
                return BadRequest("La contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula y un número");

            // Verificar si el nombre de usuario ya existe
            var userExistsByName = await _userManager.FindByNameAsync(model.Name);
            if (userExistsByName != null)
                return BadRequest("El nombre de usuario ya existe");

            // Verificar si el email ya existe
            var userExistsByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userExistsByEmail != null)
                return BadRequest("El correo electrónico ya está registrado");

            var user = new IdentityUser
            {
                UserName = model.Name,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Usuario registrado correctamente ✅");
        }

        // 🔹 LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Todos los campos son obligatorios");

            var user = await _userManager.FindByNameAsync(model.Name); // buscamos por nombre

            if (user == null)
                return Unauthorized("Usuario o contraseña incorrectos");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Usuario o contraseña incorrectos");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        // 🔹 PROFILE - Cambiar nombre
        [Authorize]
        [HttpPut("update-username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewName))
                return BadRequest(new { message = "El nombre no puede estar vacío" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            // Verificar si el nuevo nombre ya existe
            var exists = await _userManager.FindByNameAsync(model.NewName);
            if (exists != null) return BadRequest(new { message = "El nombre ya está en uso" });

            user.UserName = model.NewName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return StatusCode(500, new { message = "Error al actualizar el usuario" });

            return Ok(new { message = "Nombre de usuario actualizado con éxito ✅" });
        }

        // 🔹 PROFILE - Eliminar cuenta
        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return StatusCode(500, new { message = "Error al eliminar la cuenta" });

            return Ok(new { message = "Cuenta eliminada con éxito ✅" });
        }

        // 🔹 Generador de token JWT
        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // 🔹 Validación de contraseña
        private bool ValidatePassword(string password)
        {
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
            return regex.IsMatch(password);
        }
    }

    public record RegisterModel(string Name, string Email, string Password);
    public record LoginModel(string Name, string Password);
    public record UpdateUsernameModel(string NewName);
}
