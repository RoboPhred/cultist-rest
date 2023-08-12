
using System;
using Ceen;

namespace CSRestAPI.Server.Exceptions
{
    public class WebException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public WebException(HttpStatusCode statusCode, string message)
        : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}