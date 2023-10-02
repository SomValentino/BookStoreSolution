using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer.Helpers;
using IdentityServer.ViewModels.Admin;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.Models;
using IdentityServerHost.Quickstart.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityServer.Controllers {
    [Authorize (Roles = "Administrator")]
    public class AdminController : Controller {
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminController> _logger;

        public AdminController (IMapper mapper, ConfigurationDbContext configurationDbConext, ILogger<AdminController> logger) {
            _configurationDbContext = configurationDbConext;
            _mapper = mapper;
            _logger = logger;
        }
        // GET: /<controller>/
        public IActionResult Index () {
            return View ();
        }

        [Route ("Admin/CreateApiScope")]
        [HttpPost]
        public async Task<JsonResult> CreateApiScope (ApiScopeViewModel apiScopeViewModel) {
            try {
                var existinApiScope = await _configurationDbContext.ApiScopes.FirstOrDefaultAsync (x => x.Name.ToLower () == apiScopeViewModel.Name.ToLower ());

                if (existinApiScope != null) throw new IdentityServerException ("There is an api scope with same name that already exist");

                var apiScope = new IdentityServer4.EntityFramework.Entities.ApiScope ();
                //IdentityServerConstants.StandardScopes.

                _mapper.Map (apiScopeViewModel, apiScope);

                await _configurationDbContext.ApiScopes.AddAsync (apiScope);

                await _configurationDbContext.SaveChangesAsync ();

                return Json (new { Success = true, Error = string.Empty, Message = "Api scope successfully created" },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            } catch (IdentityServerException ex) {
                _logger.LogError (ex.Message, ex);
                return Json (new { Success = false, Error = ex.Message, Message = string.Empty },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            } catch (Exception ex) {
                _logger.LogError (ex.Message, ex);
                return Json (new { Success = false, Error = "Error occured. Please contact support", Message = string.Empty },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            }
        }

        [Route ("Admin/GetApiScopes")]
        [HttpPost]
        public async Task<JsonResult> GetApiScopes (string searchTerm = null) {
            try {
                var apiscopes = new List<IdentityServer4.EntityFramework.Entities.ApiScope> ();
                if (string.IsNullOrEmpty (searchTerm))
                    apiscopes = await _configurationDbContext.ApiScopes.ToListAsync ();
                else apiscopes = await _configurationDbContext.ApiScopes.Where (x => x.Name.ToLower ().Contains (searchTerm.ToLower ())).ToListAsync ();

                return Json (apiscopes.Select (x => new { id = x.Id, text = x.Name }),
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });

            } catch (Exception ex) {
                _logger.LogError (ex.Message, ex);
                return Json (new { Success = false, Error = "Error occured. Please contact support", Message = string.Empty },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            }
        }

        [Route ("Admin/GetGrantTypes")]
        [HttpPost]
        public async Task<JsonResult> GetGrantTypes (string searchTerm = null) {
            try {
                var grantTypes = new List<KeyValuePair<GrantTypeEnum, string>> ();

                var grants = string.Empty;

                if (!string.IsNullOrEmpty (searchTerm))
                    grantTypes = grants.GetGrantTypeMapping ()
                    .Where (x => x.Key.ToString ().ToLower ().Contains (searchTerm)).ToList ();
                else grantTypes = grants.GetGrantTypeMapping ().ToList ();

                return Json (grantTypes.Select (x => new { id = x.Key, text = x.Key.ToString () }),
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            } catch (Exception ex) {
                _logger.LogError (ex.Message, ex);
                return Json (new { Success = false, Error = "Error occured. Please contact support", Message = string.Empty },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            }
        }

        [Route ("Admin/CreateClient")]
        [HttpPost]
        public async Task<JsonResult> CreateClient (ClientViewModel clientViewModel) {
            try {
                var clientExist = await _configurationDbContext.Clients
                    .FirstOrDefaultAsync (x => x.ClientId == clientViewModel.ClientId);

                if (clientExist != null) throw new IdentityServerException ("Client already exist");

                var scopeList = JsonConvert.DeserializeObject<List<int>> (clientViewModel.ScopeList);

                var grantList = JsonConvert.DeserializeObject<List<int>> (clientViewModel.GrantTypeList);

                var scopes = await _configurationDbContext.ApiScopes
                    .Where (x => scopeList.Contains (x.Id)).ToListAsync ();

                if (!scopes.Any ()) throw new IdentityServerException ("No scopes found");

                var grantTypes = string.Empty.GetGrantTypeMapping ()
                    .Where (x => grantList.Contains ((int) x.Key))
                    .Select (y => y.Value).ToList ();

                if (!grantTypes.Any ()) throw new IdentityServerException ("No grant type found");

                var clientSecret = new ClientSecret ();

                clientSecret.Value = clientViewModel.ClientSecret;
                clientSecret.Description = clientViewModel.ClientName + " Secret";
                clientSecret.Created = DateTime.UtcNow;
                clientSecret.Type = "Secret";

                var client = new IdentityServer4.EntityFramework.Entities.Client ();

                client.ClientName = clientViewModel.ClientName;
                client.ClientId = clientViewModel.ClientId;
                client.Enabled = true;
                client.RequireClientSecret = clientViewModel.RequireClientSecret;
                client.RequireConsent = clientViewModel.RequireConscent;
                client.RequirePkce = clientViewModel.RequirePKCE;
                client.AllowAccessTokensViaBrowser = clientViewModel.AllowAccessTokenViaBrowser;
                client.AllowOfflineAccess = clientViewModel.AllowOfflineAccess;
                client.RequireRequestObject = false;
                client.AllowPlainTextPkce = clientViewModel.RequirePKCE;
                client.IdentityTokenLifetime = 86000;
                client.AccessTokenLifetime = 86000;
                client.AuthorizationCodeLifetime = 86000;
                client.AbsoluteRefreshTokenLifetime = 86000;
                client.SlidingRefreshTokenLifetime = 86000;
                client.RefreshTokenUsage = 86000;
                client.UpdateAccessTokenClaimsOnRefresh = true;
                client.RefreshTokenExpiration = 86000;
                client.AccessTokenType = (int) AccessTokenType.Reference;
                client.EnableLocalLogin = true;
                client.IncludeJwtId = true;
                client.AlwaysSendClientClaims = true;
                client.Created = DateTime.UtcNow;
                client.DeviceCodeLifetime = 86000;
                client.NonEditable = false;
                client.FrontChannelLogoutUri = clientViewModel.FrontChannelUri;
                client.BackChannelLogoutUri = clientViewModel.BackChannelUri;
                client.FrontChannelLogoutSessionRequired = !string.IsNullOrEmpty (clientViewModel.FrontChannelUri);
                client.BackChannelLogoutSessionRequired = !string.IsNullOrEmpty (clientViewModel.BackChannelUri);
                client.ClientSecrets = new List<ClientSecret> { clientSecret };
                client.AllowedGrantTypes = grantTypes.Select (x => new ClientGrantType {
                    GrantType = x
                }).ToList ();
                client.AllowedScopes = scopes.Select (x => new ClientScope {
                    Scope = x.Name
                }).ToList ();
                client.RedirectUris = new List<ClientRedirectUri> { new ClientRedirectUri { RedirectUri = clientViewModel.ClientRedirectUri } };
                client.PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri> { new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = string.IsNullOrEmpty (clientViewModel.ClientPostRedirectUri) ? clientViewModel.ClientRedirectUri : clientViewModel.ClientPostRedirectUri } };

                var clientCreated = await _configurationDbContext.Clients.AddAsync (client);

                await _configurationDbContext.SaveChangesAsync ();

                return Json (new { Success = true, Error = string.Empty, Message = "Client successfully created" },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            } catch (IdentityServerException ex) {
                _logger.LogError (ex.Message, ex);
                return Json (new { Success = false, Error = ex.Message, Message = string.Empty },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            } catch (Exception ex) {
                _logger.LogError (ex.Message, ex);
                return Json (new { Success = false, Error = "Error occured. Please contact support", Message = string.Empty },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            }
        }

        [Route ("Admin/CreateApiResource")]
        [HttpPost]
        public async Task<JsonResult> CreateApiResource (ApiResourceViewModal apiResourceViewModal) {
            try {
                var scopeList = JsonConvert.DeserializeObject<List<int>> (apiResourceViewModal.ScopeList);

                List<ApiResourceScope> scopes = null;

                if (scopeList.Any ()) {
                    scopes = await _configurationDbContext.ApiScopes
                        .Where (x => scopeList.Contains (x.Id)).Select (y =>
                            new ApiResourceScope {
                                Scope = y.Name
                            }).ToListAsync ();
                }

                var apiResourceSecret = new IdentityServer4.EntityFramework.Entities.ApiResourceSecret {
                    Value = apiResourceViewModal.Secret.Sha256(),
                    Type = "SharedSecret",
                    Description = apiResourceViewModal.Description,
                    Created = DateTime.UtcNow
                };

                var apiResource = new IdentityServer4.EntityFramework.Entities.ApiResource {
                    Name = apiResourceViewModal.Name,
                    DisplayName = apiResourceViewModal.DisplayName,
                    Description = apiResourceViewModal.Description,
                    Enabled = apiResourceViewModal.Enabled,
                    ShowInDiscoveryDocument = apiResourceViewModal.ShowInDiscoveryDocument,
                    Created = DateTime.UtcNow,
                    NonEditable = false,
                    Scopes = scopes,
                    Secrets = new List<ApiResourceSecret> {apiResourceSecret}
                };

                await _configurationDbContext.ApiResources.AddAsync (apiResource);

                await _configurationDbContext.SaveChangesAsync();

                return Json (new { Success = true, Error = string.Empty, Message = "Api Resource successfully created" },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            } catch (Exception ex) {
                _logger.LogError (ex.Message, ex);
                return Json (new { Success = false, Error = "Error occured. Please contact support", Message = string.Empty },
                    new JsonSerializerOptions () { PropertyNamingPolicy = null });
            }
        }
    }
}