using System;
using System.Collections.Generic;
using IdentityServer.Helpers;
using IdentityServer4.Models;
using IdentityServerHost.Quickstart.UI.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerHost.Quickstart.UI.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Checks if the redirect URI is for a native client.
        /// </summary>
        /// <returns></returns>
        public static bool IsNativeClient(this AuthorizationRequest context)
        {
            return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
               && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
        }

        public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri)
        {
            controller.HttpContext.Response.StatusCode = 200;
            controller.HttpContext.Response.Headers["Location"] = "";
            
            return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
        }

        public static Dictionary<GrantTypeEnum,string> GetGrantTypeMapping(this string grantType)
        {
            return new Dictionary<GrantTypeEnum, string>()
            {
                {GrantTypeEnum.Implicit, GrantType.Implicit },
                {GrantTypeEnum.AuthorizationCode, GrantType.AuthorizationCode },
                {GrantTypeEnum.ClientCredentials, GrantType.ClientCredentials },
                {GrantTypeEnum.DeviceFlow, GrantType.DeviceFlow },
                {GrantTypeEnum.Hybrid, GrantType.Hybrid },
                {GrantTypeEnum.ResourceOwnerPassword, GrantType.ResourceOwnerPassword}
            };
        }
    }
}
