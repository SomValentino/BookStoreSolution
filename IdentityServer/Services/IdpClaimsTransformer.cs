using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Services {
    public class IdpClaimsTransformer : IClaimsTransformation {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdpClaimsTransformer (UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync (ClaimsPrincipal principal) {
            var identity = ((ClaimsIdentity) principal.Identity);

            var claims = new List<Claim> ();

            var user = await _userManager.FindByNameAsync (identity.Name);

            var roles = await _userManager.GetRolesAsync (user);

            claims.AddRange (new List<Claim> () {
                new Claim (JwtClaimTypes.Subject, user.Id),
                new Claim (JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim (JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim ("security_stamp", user.SecurityStamp)
            });

            claims.AddRange (roles.Select (role => new Claim (JwtClaimTypes.Role, role)));

            identity.AddClaims (claims);

            return await Task.FromResult (principal);
        }
    }
}