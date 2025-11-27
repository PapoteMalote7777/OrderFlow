using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaGod.Identity.Data
{
    public static class RoleSeeder
    {
        private const string DefaultAdminUserName = "Admin";
        private const string DefaultAdminEmail = "admin@gmail.com";
        private const string DefaultAdminPassword = "Adm1n!strad0rFach3r0";

        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            var roles = new[] { "User", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var createRoleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!createRoleResult.Succeeded)
                    {
                        Console.WriteLine($"RoleSeeder: fallo al crear rol '{role}': {string.Join(", ", createRoleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }

            var adminUserName = config["Seed:AdminUserName"]?.Trim();
            var adminEmail = config["Seed:AdminEmail"]?.Trim();
            var adminPassword = config["Seed:AdminPassword"];
            var useDefaults = string.IsNullOrWhiteSpace(adminUserName) || string.IsNullOrWhiteSpace(adminPassword);
            if (useDefaults)
            {
                Console.WriteLine("RoleSeeder: faltan valores Seed:Admin* en la configuración; se usarán valores por defecto embebidos.");
                adminUserName ??= DefaultAdminUserName;
                adminEmail ??= DefaultAdminEmail;
                adminPassword ??= DefaultAdminPassword;
            }

            IdentityUser admin = await userManager.FindByNameAsync(adminUserName);
            if (admin == null && !string.IsNullOrEmpty(adminEmail))
            {
                admin = await userManager.FindByEmailAsync(adminEmail);
            }

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                {
                    Console.WriteLine($"RoleSeeder: fallo al crear admin '{adminUserName}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                    return;
                }

                Console.WriteLine($"RoleSeeder: admin '{adminUserName}' creado correctamente.");
            }
            else
            {
                var needUpdate = false;
                if (admin.UserName != adminUserName)
                {
                    admin.UserName = adminUserName;
                    needUpdate = true;
                }

                if (!string.IsNullOrEmpty(adminEmail) && admin.Email != adminEmail)
                {
                    admin.Email = adminEmail;
                    admin.EmailConfirmed = true;
                    needUpdate = true;
                }

                if (!admin.EmailConfirmed)
                {
                    admin.EmailConfirmed = true;
                    needUpdate = true;
                }

                if (needUpdate)
                {
                    var updateResult = await userManager.UpdateAsync(admin);
                    if (!updateResult.Succeeded)
                    {
                        Console.WriteLine($"RoleSeeder: fallo al actualizar admin existente '{adminUserName}': {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                    }
                    else
                    {
                        Console.WriteLine($"RoleSeeder: admin existente '{adminUserName}' actualizado.");
                    }
                }

                var hasPassword = await userManager.HasPasswordAsync(admin);
                if (!hasPassword)
                {
                    var addPwdResult = await userManager.AddPasswordAsync(admin, adminPassword);
                    if (!addPwdResult.Succeeded)
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(admin);
                        var resetResult = await userManager.ResetPasswordAsync(admin, token, adminPassword);
                        if (!resetResult.Succeeded)
                        {
                            Console.WriteLine($"RoleSeeder: no se pudo establecer la contraseña para '{adminUserName}': {string.Join(", ", addPwdResult.Errors.Concat(resetResult.Errors).Select(e => e.Description))}");
                        }
                        else
                        {
                            Console.WriteLine($"RoleSeeder: contraseña establecida para admin existente '{adminUserName}' mediante reset.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"RoleSeeder: contraseña añadida a admin existente '{adminUserName}'.");
                    }
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(admin, "Admin");
                if (!addRoleResult.Succeeded)
                {
                    Console.WriteLine($"RoleSeeder: fallo al asignar rol 'Admin' a '{adminUserName}': {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
                }
                else
                {
                    Console.WriteLine($"RoleSeeder: rol 'Admin' asignado a '{adminUserName}'.");
                }
            }

            Console.WriteLine($"RoleSeeder: Admin final -> UserName='{admin.UserName}', Email='{admin.Email}'");
        }
    }
}
