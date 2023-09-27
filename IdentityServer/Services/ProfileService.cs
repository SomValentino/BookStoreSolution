using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfileService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;

            var user = await _userManager.FindByIdAsync(subjectId);

            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var claims = await GetClaimsFromUser(user, context);
            context.IssuedClaims = claims.ToList();
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUser(ApplicationUser user,ProfileDataRequestContext context)
        {
            var claims = new List<Claim>();

            var roles = await _userManager.GetRolesAsync(user);

            if(context.RequestedClaimTypes.Any(claim => claim == JwtClaimTypes.Subject))
            {
                claims.AddRange(new List<Claim>(){
                    new Claim(JwtClaimTypes.Subject,user.Id),
                    new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                });
            }

            if(context.RequestedClaimTypes.Any(claim => claim == JwtClaimTypes.Profile))
            {
                if (!string.IsNullOrWhiteSpace(user.FirstName))
                    claims.Add(new Claim(nameof(user.FirstName), user.FirstName));
                if (!string.IsNullOrWhiteSpace(user.LastName))
                    claims.Add(new Claim(nameof(user.LastName), user.LastName));
                if (!string.IsNullOrWhiteSpace(user.City))
                    claims.Add(new Claim(nameof(user.City), user.City));
                if (!string.IsNullOrWhiteSpace(user.Country))
                    claims.Add(new Claim(nameof(user.Country), user.Country));
                if (!string.IsNullOrWhiteSpace(user.State))
                    claims.Add(new Claim(nameof(user.State), user.State));
                if (!string.IsNullOrWhiteSpace(user.Street))
                    claims.Add(new Claim(nameof(user.Street), user.Street));
                if (!string.IsNullOrWhiteSpace(user.ZipCode))
                    claims.Add(new Claim(nameof(user.ZipCode), user.ZipCode));

                if (_userManager.SupportsUserEmail)
                {
                    claims.AddRange(new[]
                    {
                      new Claim(JwtClaimTypes.Email, user.Email),
                      new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false",ClaimValueTypes.Email)
                    });
                }

                if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    claims.AddRange(new[]
                    {
                        new Claim(JwtClaimTypes.PhoneNumber,user.PhoneNumber),
                        new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true": "false", ClaimValueTypes.Boolean)
                    });
                }
            }


            if (roles != null && roles.Any())
            {
                claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            }


            return claims;

        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp")
                        .Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.Now;
            }
        }
    }
}
