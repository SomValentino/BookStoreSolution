using System;
using IdentityServer4.Events;

namespace IdentityServer.Events
{
    public class RegisterUserFaliureEvent : Event
    {
        public RegisterUserFaliureEvent(string username, string error, bool interactive = true, string clientId = null)
           : base("Authentication", "Register User Failure", EventTypes.Failure, 1001, error)
        {

        }
    }
}
