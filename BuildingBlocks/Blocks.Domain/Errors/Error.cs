namespace Blocks.Domain.Errors;

public sealed record Error(ErrorCode Code, string Message, string? Field = null)
{
    public int StatusCode => Code switch
    {
        ErrorCode.NotFound     => 404,
        ErrorCode.Conflict     => 409,
        ErrorCode.Unauthorized => 401,
        ErrorCode.Forbidden    => 403,
        ErrorCode.Validation   => 422,
        ErrorCode.Internal     => 500,
        _                      => 500
    };

    public static Error NotFound(string message) => new(ErrorCode.NotFound, message);
    public static Error Conflict(string message) => new(ErrorCode.Conflict, message);
    public static Error Unauthorized(string message) => new(ErrorCode.Unauthorized, message);
    public static Error Forbidden(string message) => new(ErrorCode.Forbidden, message);
    public static Error Validation(string message, string? field = null) => new(ErrorCode.Validation, message, field);
    public static Error Internal(string message) => new(ErrorCode.Internal, message);
}
