using Blocks.Contracts.Common;
using Blocks.Domain.Errors;

namespace Blocks.Contracts.Http;

public sealed class FloweryApiResponse<T>
{
    public T? Data { get; init; }
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public string MessageLocalized { get; init; } = string.Empty;
    public string StatusCode { get; init; } = "Success";

    public static FloweryApiResponse<T> Success(
        T? data,
        string message = "Success",
        string? messageLocalized = null,
        string statusCode = "Success") =>
        new()
        {
            Data = data,
            IsSuccess = true,
            Message = message,
            MessageLocalized = messageLocalized ?? message,
            StatusCode = statusCode
        };

    public static FloweryApiResponse<T> Failure(
        string message,
        string statusCode = "BadRequest",
        string? messageLocalized = null,
        T? data = default) =>
        new()
        {
            Data = data,
            IsSuccess = false,
            Message = message,
            MessageLocalized = messageLocalized ?? message,
            StatusCode = statusCode
        };

    public static FloweryApiResponse<T> FromResult(
        Result<T> result,
        string successMessage = "Success",
        string? successMessageLocalized = null,
        string successStatusCode = "Success")
    {
        if (result.IsSuccess)
        {
            return Success(result.Value, successMessage, successMessageLocalized, successStatusCode);
        }

        var error = result.Error;
        var status = error?.StatusCode switch
        {
            404 => "NotFound",
            401 => "Unauthorized",
            403 => "Forbidden",
            409 => "Conflict",
            422 => "ValidationError",
            _ => "BadRequest"
        };

        return Failure(error?.Message ?? "An error occurred.", status, error?.Message, default);
    }
}
