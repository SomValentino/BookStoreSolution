using System;
namespace IdentityServer.ViewModels.Admin
{
    public class ApiResourceViewModal
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string ScopeList { get; set; }
        public bool Enabled { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
    }
}
