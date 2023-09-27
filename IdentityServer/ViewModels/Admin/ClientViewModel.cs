using System;
using System.Collections.Generic;

namespace IdentityServer.ViewModels.Admin
{
    public class ClientViewModel
    {
        public string ClientName { get; set; }
        public string ClientSecret { get; set; }
        public string ClientId { get; set; }
        public string FrontChannelUri { get; set; }
        public string BackChannelUri { get; set; }
        public string ScopeList { get; set; }
        public string GrantTypeList { get; set; }
        public bool RequireClientSecret { get; set; }
        public bool RequireConscent { get; set; }
        public bool RequirePKCE { get; set; }
        public bool AllowAccessTokenViaBrowser { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public string ClientRedirectUri { get; set; }
        public string ClientPostRedirectUri { get; set; }
    }
}
    