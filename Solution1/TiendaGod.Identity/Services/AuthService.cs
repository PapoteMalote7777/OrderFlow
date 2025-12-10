using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TiendaGod.Shared.Events;

namespace TiendaGod.Identity.Services
{
    public class AuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            IPublishEndpoint publishEndpoint)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<string> RegisterAsync(string name, string email, string password)
        {
            var userExistsByName = await _userManager.FindByNameAsync(name);
            if (userExistsByName != null)
                throw new InvalidOperationException("El nombre de usuario ya existe");

            var userExistsByEmail = await _userManager.FindByEmailAsync(email);
            if (userExistsByEmail != null)
                throw new InvalidOperationException("El correo electrónico ya está registrado");

            var user = new IdentityUser
            {
                UserName = name,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "User");

            await _publishEndpoint.Publish(new UserRegisteredEvent(
                UserId: user.Id,
                UserName: user.UserName,
                Email: user.Email
            ));

            return "Usuario registrado correctamente ✅";
        }

        public async Task<string> LoginAsync(string name, string password)
        {
            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            return await GenerateJwtToken(user);
        }

        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
    }
}
