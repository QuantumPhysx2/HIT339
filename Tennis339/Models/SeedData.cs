using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tennis339.Models
{
    public class SeedData
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration _configuration)
        {
            // Instantiate reference to RoleManager and UserManager API
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Creating roles
            string[] roles = {
                "Admin",
                "Coach",
                "Member"
            };

            // Instantiate Identity operations var
            IdentityResult _identityResult;

            foreach (var role in roles)
            {
                // Check existing roles
                var roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    // Create new role
                    _identityResult = await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Setup default Admin account
            var superAdmin = new IdentityUser
            {
                // Read from 'appsettings.json'
                UserName = _configuration.GetSection("AppSettings")["UserEmail"],
                Email = _configuration.GetSection("AppSettings")["UserEmail"]
            };

            string password = _configuration.GetSection("AppSettings")["UserPassword"];

            // Check existing Admin account
            var user = await _userManager.FindByEmailAsync(_configuration.GetSection("AppSettings")["UserEmail"]);
            if (user == null)
            {
                // Create non-existing Admin account
                var createSuperAdmin = await _userManager.CreateAsync(superAdmin, password);

                if (createSuperAdmin.Succeeded)
                {
                    // Auto-assign 'Admin' user to 'Admin' role
                    await _userManager.AddToRoleAsync(superAdmin, "Admin");
                }
            }
        }
    }
}
