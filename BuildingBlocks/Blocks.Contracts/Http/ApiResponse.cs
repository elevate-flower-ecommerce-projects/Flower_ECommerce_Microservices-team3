using Blocks.Contracts.Common;
using Blocks.Domain.Errors;

namespace Blocks.Contracts.Http;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public ApiError? Error { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "Request completed successfully") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(Error error) =>
        new() { Success = false, Message = error.Message, Error = new ApiError(error.Code.ToString(), error.Field) };

    public static ApiResponse<T> FromResult(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : Fail(result.Error!);
}

public sealed record ApiError(string Code, string? Field);
