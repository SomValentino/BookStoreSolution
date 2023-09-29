using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Services {
    public class ProfileService : IProfileService {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfileService (UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager) {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync (ProfileDataRequestContext context) {
            var subject = context.Subject ??
                throw new ArgumentNullException (nameof (context.Subject));

            var subjectId = subject.Claims.Where (x => x.Type == "sub").FirstOrDefault ().Value;
            var user = await _userManager.FindByIdAsync (subjectId);

            if (user == null)
                throw new ArgumentException ("Invalid subject identifier");

            var claims = await GetClaimsFromUser (user, context);
            context.IssuedClaims = claims.ToList ();
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUser (ApplicationUser user, ProfileDataRequestContext context) {
            var claims = new List<Claim> ();

            var roles = await _userManager.GetRolesAsync (user);

            if (context.RequestedClaimTypes.Any (claim => claim == IdentityServer4.IdentityServerConstants.StandardScopes.OpenId)) {
                claims.Add (
                    new Claim (JwtClaimTypes.Subject, user.Id)
                );
            }

            if (context.RequestedClaimTypes.Any (claim => claim == "security_stamp")) {
                claims.Add (
                    new Claim ("security_stamp", user.SecurityStamp)
                );
            }

            if (context.RequestedClaimTypes.Any (claim => claim == IdentityServer4.IdentityServerConstants.StandardScopes.Profile)) {
                claims.AddRange (new List<Claim> {
                    new Claim (JwtClaimTypes.PreferredUserName, user.UserName),
                    new Claim (JwtRegisteredClaimNames.UniqueName, user.UserName)
                });
            }
            if (_userManager.SupportsUserEmail) {
                claims.AddRange (new [] {
                    new Claim (JwtClaimTypes.Email, user.Email),
                        new Claim (JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Email)
                });
            }

            if (roles != null && roles.Any ()) {
                claims.AddRange (roles.Select (role => new Claim (JwtClaimTypes.Role, role)));
            }

            return claims;

        }

        public async Task IsActiveAsync (IsActiveContext context) {
            var subject = context.Subject ??
                throw new ArgumentNullException (nameof (context.Subject));

            var subjectId = subject.Claims.Where (x => x.Type == "sub").FirstOrDefault ().Value;
            var user = await _userManager.FindByIdAsync (subjectId);

            context.IsActive = false;

            if (user != null) {
                var security_stamp_changed = false;
                if (_userManager.SupportsUserSecurityStamp) {
                    var security_stamp = subject.Claims.Where (c => c.Type == "security_stamp")
                        .Select (c => c.Value).SingleOrDefault ();
                    if (security_stamp != null) {
                        var latest_security_stamp = await _userManager.GetSecurityStampAsync (user);
                        security_stamp_changed = security_stamp != latest_security_stamp;
                    }
                }

                context.IsActive = !security_stamp_changed && !await _userManager.IsLockedOutAsync (user);
            }
        }
    }
}