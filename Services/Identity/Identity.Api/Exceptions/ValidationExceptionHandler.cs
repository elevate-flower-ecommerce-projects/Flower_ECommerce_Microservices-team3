using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Api.Exceptions
{
    public class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger, IHostEnvironment env) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (httpContext.Response.HasStarted)
            {
                return false;
            }

            if (exception is FluentValidation.ValidationException validationException)
            {
                var errorMessage = string.Join(" | ", validationException.Errors
                    .Select(e => string.IsNullOrWhiteSpace(e.PropertyName)
                        ? e.ErrorMessage
                        : $"{e.PropertyName}: {e.ErrorMessage}")
                    .Distinct(StringComparer.Ordinal));

                httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                await httpContext.Response.WriteAsJsonAsync(
                    new { code = StatusCodes.Status422UnprocessableEntity, error = errorMessage },
                    cancellationToken);

                return true;
            }

            logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    status = StatusCodes.Status500InternalServerError,
                    error = exception.Message,
                    innerError = exception.InnerException?.Message,
                    details = env.IsDevelopment() ? exception.StackTrace : null
                },
                cancellationToken);

            return true;
        }
    }
}
