using Microsoft.AspNetCore.Diagnostics;

namespace Identity.Api.Exceptions
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        private const string RequestLevelErrorKey = "request";

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not FluentValidation.ValidationException validationException)
            {
                return false;
            }

            if (httpContext.Response.HasStarted)
            {
                return false;
            }

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
    }
}
