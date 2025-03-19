using ManageTask.Application.Abstractions.Auth;
using ManageTask.Infrastructure.ServiceRegistration.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace ManageTask.Infrastructure.Auth
{
    public class TokenStorage(IDistributedCache redisCache, IOptions<JwtOptions> options) : ITokenStorage
    {
        private readonly string keyFormat = "refresh-token:{0}";
        private readonly IDistributedCache redisCache = redisCache;
        private readonly TimeSpan tokenLifeTime = TimeSpan.FromHours(options.Value.RefreshTokenExpirationHours);
        public async Task<Result> DeleteTokenAsync(string token, CancellationToken cancellationToken)
        {
            var key = string.Format(keyFormat, token);
            await redisCache.RemoveAsync(key, cancellationToken);
            return Result.Success();
        }

        public async Task<Result<(string token, Guid userId)>> GetTokenAsync(string token, CancellationToken cancellationToken)
        {
            var key = string.Format(keyFormat, token);
            var storedToken = await redisCache.GetStringAsync(key, cancellationToken);
            if (storedToken is null)
            {
                return Error.Unauthorized("Срок действия токена истёк");
            }
            if(!Guid.TryParse(storedToken, out var UserId))
            {
                return Error.Failure("Некорректный формат токена");
            }
            return (token, UserId);
        }

        public async Task<Result> SetTokenAsync(string token, Guid userId, CancellationToken cancellationToken)
        {
            var key = string.Format(keyFormat, token);
            await redisCache.SetStringAsync(key, userId.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = tokenLifeTime,
            }, cancellationToken);
            return Result.Success();
        }
    }
}
