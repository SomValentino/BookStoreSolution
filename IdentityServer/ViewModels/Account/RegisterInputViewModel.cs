using System;
using System.ComponentModel.DataAnnotations;
using IdentityServerHost.Quickstart.UI.ViewModels.Account;

namespace IdentityServer.ViewModels.Account
{
    public class RegisterInputViewModel : LoginInputModel
    {
        [EmailAddress]
        public string Email { get; set; }
        [Compare(nameof(Password),ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

    }
}
