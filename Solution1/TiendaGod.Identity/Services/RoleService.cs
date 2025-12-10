using Microsoft.AspNetCore.Identity;

namespace TiendaGod.Identity.Services
{
    public class RolesService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AssignRoleAsync(string userName, string role)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado");

            if (!await _roleManager.RoleExistsAsync(role))
                throw new InvalidOperationException("Rol inválido");

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                var res = await _userManager.AddToRoleAsync(user, role);
                if (!res.Succeeded)
                    throw new InvalidOperationException(string.Join(", ", res.Errors.Select(e => e.Description)));
            }
        }

        public async Task RemoveRoleAsync(string userName, string role)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado");

            if (!await _roleManager.RoleExistsAsync(role))
                throw new InvalidOperationException("Rol inválido");

            if (await _userManager.IsInRoleAsync(user, role))
            {
                var res = await _userManager.RemoveFromRoleAsync(user, role);
                if (!res.Succeeded)
                    throw new InvalidOperationException(string.Join(", ", res.Errors.Select(e => e.Description)));
            }
        }

        public async Task<List<UserWithRolesDto>> ListUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserWithRolesDto>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new UserWithRolesDto
                {
                    UserName = u.UserName ?? "",
                    Email = u.Email ?? "",
                    Roles = roles.ToList()
                });
            }

            return result;
        }
    }

    public class UserWithRolesDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
