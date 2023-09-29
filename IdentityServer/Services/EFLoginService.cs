using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer.Services.Interfaces;
using IdentityServer.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Services
{
    public class EFLoginService : ILoginService<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EFLoginService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            
        }

        public async Task<bool> CreateUserAsync(RegisterInputViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            return result.Succeeded;
        }

        public async Task<ApplicationUser> FindByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> FindByUserClaims(ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }

        public async Task<ApplicationUser> FindByUsername(string user)
        {
            return await _userManager.FindByNameAsync(user);
        }

        public async Task RefreshSignInAsync(ApplicationUser applicationUser)
        {
            await _signInManager.RefreshSignInAsync(applicationUser);
        }

        public Task SignIn(ApplicationUser user)
        {
            return _signInManager.SignInAsync(user, true);
        }

        public Task SignInAsync(ApplicationUser user, AuthenticationProperties properties, string authenticationMethod = null)
        {
            return _signInManager.SignInAsync(user, properties, authenticationMethod);
        }

        public async Task UpdateSecurityManagerAsync(ApplicationUser applicationUser)
        {
            await _userManager.UpdateSecurityStampAsync(applicationUser);
        }

        public async Task UpdateUserAsync(ApplicationUser applicationUser)
        {
            await _userManager.UpdateAsync(applicationUser);
        }

        public async Task<bool> ValidateCredentials(string email, string password, bool rememberMe,bool lockoutOnfailure =false)
        {
            var result = await _signInManager.PasswordSignInAsync(email,password,rememberMe,lockoutOnfailure);

            return result.Succeeded;
        }

        
    }
}