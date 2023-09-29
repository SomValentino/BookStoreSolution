using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer.Services.Interfaces
{
    public interface ILoginService<T>
    {
        Task<bool> ValidateCredentials(string email, string password, bool rememberMe,bool lockoutOnfailure =false);

        Task<T> FindByUsername(string user);

        Task<T> FindByUserClaims(ClaimsPrincipal user);

        Task<T> FindByEmail(string email);

        Task SignIn(T user);

        Task SignInAsync(T user, AuthenticationProperties properties, string authenticationMethod = null);

        Task<bool> CreateUserAsync(RegisterInputViewModel model);

        Task UpdateSecurityManagerAsync(ApplicationUser applicationUser);

        Task RefreshSignInAsync(ApplicationUser applicationUser);

    }
}
