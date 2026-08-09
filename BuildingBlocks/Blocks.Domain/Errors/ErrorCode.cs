namespace Blocks.Domain.Errors;

public enum ErrorCode
{
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Validation,
    Internal,
    TooManyRequests
}
