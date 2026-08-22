using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Identity.Api.Authorization;

public class AdminAuthorizationMiddlewareResultHandler(
    ILogger<AdminAuthorizationMiddlewareResultHandler> logger)
    : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _default = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var path = context.Request.Path;
            var user = context.User.Identity?.Name ?? "anonymous";

            logger.LogWarning(
                "403 Forbidden — user: {User}, path: {Path}, IP: {IP}, agent: {UserAgent}",
                user, path, ip, userAgent);
        }

        await _default.HandleAsync(next, context, policy, authorizeResult);
    }
}
