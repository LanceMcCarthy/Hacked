using System;
using System.Net;

namespace Hacked.Core.Common
{
    public class PwnedApiException : Exception
    {
        public PwnedApiException(string message) 
            : base (message) { }
        
        public HttpStatusCode StatusCode { get; set; }
    }
}