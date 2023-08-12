
using Ceen;

namespace CSRestAPI.Server.Exceptions
{
    public class UnauthorizedException : WebException
    {
        public UnauthorizedException() : base(HttpStatusCode.Unauthorized, "Unauthorized.") { }

        public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message) { }
    }
}