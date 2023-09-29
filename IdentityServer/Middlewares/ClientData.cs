using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer.Data.DbContext;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Middleware {
    public static class ClientData {
        public static void UseClient (this IApplicationBuilder app) {
            // get dependencies
            var scope = app.ApplicationServices.CreateScope ();

            var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext> ();
            var applicationUserDbContext = scope.ServiceProvider.GetRequiredService<ApplicationUserDbContext> ();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration> ();
            var scopeName = configuration["ScopeName"];
            var clientName = configuration["ClientName"];
            var clientId = configuration["ClientId"];
            var TokenLifeTime = int.Parse (configuration["TokenLifeTime"]);
            var RefreshTokenLifeTime = int.Parse (configuration["RefreshTokenLifeTime"]);
            var SlidingRefreshTokenLifeTime = int.Parse (configuration["SlidingRefreshTokenLifeTime"]);
            var RefreshTokenUsage = int.Parse (configuration["RefreshTokenUsage"]);
            var RefreshTokenExpiration = int.Parse (configuration["RefreshTokenExpiration"]);

            var existApiScope = configurationDbContext.ApiScopes.FirstOrDefault (x => x.Name == scopeName);

            if (existApiScope == null) {
                var apiScope = new IdentityServer4.EntityFramework.Entities.ApiScope () {
                Enabled = true,
                Name = scopeName,
                DisplayName = scopeName,
                Description = scopeName,
                Required = true,
                ShowInDiscoveryDocument = true,
                UserClaims = new System.Collections.Generic.List<ApiScopeClaim> {
                new ApiScopeClaim () {
                Type = IdentityServer4.IdentityServerConstants.StandardScopes.OpenId
                },
                new ApiScopeClaim () {
                Type = IdentityServer4.IdentityServerConstants.StandardScopes.Profile

                },
                new ApiScopeClaim () {
                Type = "API"
                }
                }
                };

                apiScope = configurationDbContext.ApiScopes.Add (apiScope).Entity;

                configurationDbContext.SaveChanges ();

                var existClient = configurationDbContext.Clients.FirstOrDefault (x => x.ClientName == clientName);

                if (existClient == null) {
                    var clientSecret = new ClientSecret {
                    Value = configuration["AdminPassword"].Sha256 (),
                    Description = $"{clientName} Secret",
                    Created = DateTime.UtcNow,
                    Type = IdentityServer4.IdentityServerConstants.SecretTypes.SharedSecret
                    };

                    var client = new IdentityServer4.EntityFramework.Entities.Client {
                        ClientName = clientName,
                        ClientId = clientId,
                        Enabled = true,
                        RequireClientSecret = false,
                        IdentityTokenLifetime = TokenLifeTime,
                        AccessTokenLifetime = TokenLifeTime,
                        AuthorizationCodeLifetime = TokenLifeTime,
                        AbsoluteRefreshTokenLifetime = RefreshTokenLifeTime,
                        SlidingRefreshTokenLifetime = SlidingRefreshTokenLifeTime,
                        RefreshTokenUsage = RefreshTokenUsage,
                        UpdateAccessTokenClaimsOnRefresh = true,
                        RefreshTokenExpiration = RefreshTokenExpiration,
                        AccessTokenType = (int) AccessTokenType.Jwt,
                        EnableLocalLogin = true,
                        IncludeJwtId = true,
                        AlwaysSendClientClaims = true,
                        Created = DateTime.UtcNow,
                        DeviceCodeLifetime = TokenLifeTime,
                        FrontChannelLogoutUri = string.Empty,
                        BackChannelLogoutUri = string.Empty,
                        FrontChannelLogoutSessionRequired = false,
                        BackChannelLogoutSessionRequired = false,
                        ClientSecrets = new List<ClientSecret> { clientSecret },
                        AllowedGrantTypes = new [] { "password" }.Select (x => new ClientGrantType {
                        GrantType = x
                        }).ToList (),
                        AllowedScopes = new [] { apiScope }.Select (x => new ClientScope {
                        Scope = x.Name
                        }).ToList (),
                        RedirectUris = new List<ClientRedirectUri> { new ClientRedirectUri { RedirectUri = "http://localhost:5010/signin-oidc" } },
                        PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri> {
                        new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = "http://localhost:5010/logout" }
                        }
                    };

                    client = configurationDbContext.Clients.Add (client).Entity;

                    configurationDbContext.SaveChanges ();
                }
            }
        }
    }
}