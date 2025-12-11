using Microsoft.AspNetCore.Identity;

namespace TiendaGod.Identity.Services
{
    public class UserAdminService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserAdminService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task UpdateUserNameAsync(string currentUserName, string newUserName)
        {
            var user = await _userManager.FindByNameAsync(currentUserName);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado");

            var existing = await _userManager.FindByNameAsync(newUserName);
            if (existing != null && existing.Id != user.Id)
                throw new InvalidOperationException("El nombre ya está en uso");

            user.UserName = newUserName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task DeleteUserAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new InvalidOperationException("Usuario no encontrado");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<List<string>> GetAdminEmailsAsync()
        {
            var users = _userManager.Users.ToList();
            var adminEmails = new List<string>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    if (!string.IsNullOrEmpty(user.Email))
                        adminEmails.Add(user.Email);
                }
            }

            return adminEmails;
        }
    }
}
