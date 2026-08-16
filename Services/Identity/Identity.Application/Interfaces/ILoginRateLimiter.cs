namespace Identity.Application.Interfaces;

public interface ILoginRateLimiter
{
    bool IsBlocked(string email, string ipAddress);
    void RecordFailure(string email, string ipAddress);
    void Reset(string email, string ipAddress);
}
