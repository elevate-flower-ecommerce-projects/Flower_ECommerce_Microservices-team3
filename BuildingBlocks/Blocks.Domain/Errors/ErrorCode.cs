namespace Blocks.Domain.Errors;

public enum ErrorCode
{
    BadRequest,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Validation,
    Internal,
    TooManyRequests
}
