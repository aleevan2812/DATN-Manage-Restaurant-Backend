﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();
        switch (exception)
        {
            case EntityErrorException entityException:
                var errors = entityException.Errors;
                problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                problemDetails.Extensions.Add("message", entityException.Message);
                problemDetails.Extensions.Add("errors", errors);
                problemDetails.Extensions.Add("statusCode", StatusCodes.Status422UnprocessableEntity);
                httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                break;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}