using System;
using static IdentityModel.OidcConstants;

namespace IdentityServer.Helpers
{
    public enum GrantTypeEnum
    {
        Implicit = 1,
        AuthorizationCode,
        Hybrid,
        ClientCredentials,
        ResourceOwnerPassword,
        DeviceFlow,
        RefreshTokens,
        ExtensionGrants
    }

    
}
