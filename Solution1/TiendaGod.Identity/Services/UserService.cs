using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using TiendaGod.Shared.Events;

namespace TiendaGod.Identity.Services
{
    public class UserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public UserService(UserManager<IdentityUser> userManager, IPublishEndpoint publishEndpoint)
        {
            _userManager = userManager;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<IdentityUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityUser?> GetUserFromClaimsAsync(ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _userManager.FindByIdAsync(userId);
        }

        public async Task UpdateUsernameAsync(IdentityUser user, string newName)
        {
            if (user.UserName == newName)
                return;

            var existing = await _userManager.FindByNameAsync(newName);
            if (existing != null && existing.Id != user.Id)
                throw new InvalidOperationException("El nombre ya está en uso");

            user.UserName = newName;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task DeleteAccountAsync(IdentityUser user)
        {
            await _publishEndpoint.Publish(new UserDeletedEvent(
                user.Id,
                user.Email!
            ));

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null;
        }

        public async Task<string?> GetUserEmailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Email;
        }
    }
}
