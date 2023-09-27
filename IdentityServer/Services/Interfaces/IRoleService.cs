using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models;

namespace IdentityServer.Services.Interfaces
{
    public interface IRoleService
    {
        Task<bool> AddUserToRole(ApplicationUser user, string rolename);

        Task<bool> IsUserInRole(ApplicationUser user, string rolename);

        Task<IEnumerable<string>> GetUserRoles(ApplicationUser user);
    }
}