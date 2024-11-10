using System.Net;

namespace Application.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(object? error, string message, HttpStatusCode statusCode) : base(message)
    {
        Error = error;
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
    public string? Message { get; set; }
    public object? Error { get; }
}