using System;
using IdentityServer4.Events;

namespace IdentityServer.Events
{
    public class RegisterUserSuccessEvent : Event
    {
        public RegisterUserSuccessEvent(string username, string error, bool interactive = true, string clientId = null)
           : base("Authentication", "Register User Success", EventTypes.Failure, 1001, error)
        {

        }
    }
}
