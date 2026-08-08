using Identity.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.Infrastructure.Services
{
    public class LoginRateLimiter(IMemoryCache cache) : ILoginRateLimiter
    {
        private const int MaxFailures = 5;
        private static readonly TimeSpan Window = TimeSpan.FromMinutes(15);

        public bool IsBlocked(string email, string ipAddress)
        {
            return GetCount(EmailKey(email)) >= MaxFailures
                || GetCount(IpKey(ipAddress)) >= MaxFailures;
        }

        public void RecordFailure(string email, string ipAddress)
        {
            Increment(EmailKey(email));
            Increment(IpKey(ipAddress));
        }

        public void Reset(string email, string ipAddress)
        {
            cache.Remove(EmailKey(email));
            cache.Remove(IpKey(ipAddress));
        }

        private int GetCount(string key)
            => cache.TryGetValue(key, out int count) ? count : 0;

        private void Increment(string key)
        {
            var count = GetCount(key) + 1;
            cache.Set(key, count, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = Window,
                Priority = CacheItemPriority.Low
            });
        }

        private static string EmailKey(string email) => $"rl:email:{email.ToLowerInvariant()}";
        private static string IpKey(string ip) => $"rl:ip:{ip}";
    }
}
