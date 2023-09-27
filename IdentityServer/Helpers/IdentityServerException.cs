using System;
namespace IdentityServer.Helpers
{
    public class IdentityServerException : Exception
    {
        public IdentityServerException(string message): base(message)
        {

        }
        public IdentityServerException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
