using System;
using System.Collections.Generic;
using System.Net;

namespace Application.Exceptions;

public class EntityErrorException : Exception
{
    public EntityErrorException(List<ValidationError> errors, string message, HttpStatusCode statusCode) : base(message)
    {
        Errors = errors;
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
    public string? Message { get; set; } = "Lỗi xảy ra khi xác thực dữ liệu...";
    public List<ValidationError> Errors { get; }
}

public class ValidationError
{
    public string Field { get; set; }
    public string Message { get; set; }

    public ValidationError(string field, string message)
    {
        Field = field;
        Message = message;
    }
}