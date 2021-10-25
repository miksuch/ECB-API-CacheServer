using System;
using System.Net;

namespace Challenge.CacheServer.Infrastructure.Models
{
    public class EcbServerMessageException : Exception
    {
        public HttpStatusCode ServerStatusCode { get; private set; }
        public EcbServerMessageException(string message, HttpStatusCode statusCode) : base(message) 
        {
            ServerStatusCode = statusCode;
        }
    }
}
