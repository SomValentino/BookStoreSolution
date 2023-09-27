using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<bool> AddUserToRole(ApplicationUser user, string rolename)
        {
            var roleExist = await _roleManager.RoleExistsAsync(rolename);

            if (!roleExist)
            {
                var identityRoleResult = await _roleManager.CreateAsync(new IdentityRole { Name = rolename });

                if (identityRoleResult.Succeeded)
                {
                    var resultRoleNotExist = await _userManager.AddToRoleAsync(user, rolename);
                    return resultRoleNotExist.Succeeded;
                }

                return false;

            }

            var result = await _userManager.AddToRoleAsync(user, rolename);
            return result.Succeeded;
        }

        public async Task<IEnumerable<string>> GetUserRoles(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsUserInRole(ApplicationUser user, string rolename)
        {
            if (user == null || string.IsNullOrEmpty(rolename)) return false;
            return await _userManager.IsInRoleAsync(user, rolename);
        }
    }
}
