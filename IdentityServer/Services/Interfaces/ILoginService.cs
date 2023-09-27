using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer.Services.Interfaces
{
    public interface ILoginService<T>
    {
        Task<bool> ValidateCredentials(T user, string password);

        Task<T> FindByUsername(string user);

        Task<T> FindByUserClaims(ClaimsPrincipal user);

        Task<T> FindByEmail(string email);

        Task SignIn(T user);

        Task SignInAsync(T user, AuthenticationProperties properties, string authenticationMethod = null);

        Task<bool> CreateUserAsync(RegisterInputViewModel model);
    }
}
