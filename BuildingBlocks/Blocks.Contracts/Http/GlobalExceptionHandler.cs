using System.Net;
using Blocks.Domain.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Blocks.Contracts.Http;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, apiResponse) = exception switch
        {
            ValidationException valEx => (
                StatusCodes.Status422UnprocessableEntity,
                new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Error = new ApiError("Validation", string.Join("; ", valEx.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")))
                }
            ),
            KeyNotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                ApiResponse<object>.Fail(Error.NotFound(notFoundEx.Message))
            ),
            UnauthorizedAccessException unauthEx => (
                StatusCodes.Status401Unauthorized,
                ApiResponse<object>.Fail(Error.Unauthorized(unauthEx.Message))
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.Fail(Error.Internal("An unexpected error occurred."))
            )
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(apiResponse, cancellationToken);

        return true;
    }
}
