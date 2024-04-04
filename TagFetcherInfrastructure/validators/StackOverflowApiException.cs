using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TagFetcherInfrastructure.validators
{
    public class StackOverflowApiException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string ErrorType { get; private set; }

        public StackOverflowApiException(string message, HttpStatusCode statusCode, string errorType = "General") : base(message)
        {
            StatusCode = statusCode;
            ErrorType = errorType;
        }

        public static StackOverflowApiException RateLimitExceeded(string message)
        {
            return new StackOverflowApiException(message, HttpStatusCode.TooManyRequests, "RateLimitExceeded");
        }

        public static StackOverflowApiException NotFound(string message)
        {
            return new StackOverflowApiException(message, HttpStatusCode.NotFound, "NotFound");
        }

        public static StackOverflowApiException BadParameter(string message)
        {
            return new StackOverflowApiException(message, HttpStatusCode.BadRequest, "BadParameter");
        }

        public static StackOverflowApiException WriteFailed(string message)
        {
            return new StackOverflowApiException(message, HttpStatusCode.ProxyAuthenticationRequired, "WriteFailed");
        }

        public static StackOverflowApiException DuplicateRequest(string message)
        {
            return new StackOverflowApiException(message, HttpStatusCode.Conflict, "DuplicateRequest");
        }

        public static StackOverflowApiException InternalError(string message)
        {
            return new StackOverflowApiException(message, HttpStatusCode.InternalServerError, "InternalError");
        }

        public static StackOverflowApiException ThrottleViolation(string message)
        {
            return new StackOverflowApiException(message, HttpStatusCode.BadGateway, "ThrottleViolation");
        }

        public static StackOverflowApiException TemporarilyUnavailable(string message)
        {
            return new StackOverflowApiException(message, HttpStatusCode.ServiceUnavailable, "TemporarilyUnavailable");
        }

    }
}
