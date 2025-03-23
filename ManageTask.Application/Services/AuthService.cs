using ManageTask.Application.Abstractions.Auth;
using ManageTask.Application.Abstractions.Services;
using ManageTask.Application.Contants;
using ManageTask.Domain;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Extensions.FunctionalExtensions.Async;
using ResultSharp.Extensions.FunctionalExtensions.Sync;
using ResultSharp.Logging;
using System.Security.Claims;

namespace ManageTask.Application.Services
{
    public class AuthService(IJwtProvider jwtProvider, ITokenStorage tokenStorage) : IAuthService
    {
        private const string BearerPrefix = "Bearer ";
        public const string AuthorizationHeader = "Authorization";
        public const string RefreshTokenHeader = "Refresh-Token";

        private readonly IJwtProvider jwtProvider = jwtProvider;
        private readonly ITokenStorage tokenStorage = tokenStorage;
        public async Task<Result> ClearTokensAsync(HttpRequest request, HttpResponse response, CancellationToken cancellationToken)
        {
            response.Headers.Remove(AuthorizationHeader);
            if (request.Headers.TryGetValue(RefreshTokenHeader, out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
            {
                response.Headers.Remove(RefreshTokenHeader);

                await tokenStorage.DeleteTokenAsync(refreshToken!, cancellationToken);
            }
            return Result.Success();
        }

        public async Task<Result<(string accessToken, string refreshToken)>> GenerateAccessRefreshPairAsync(User user, Role role, CancellationToken cancellationToken)
        {
            var accessToken = GenerateAccessToken(user, role);
            var refreshToken = await GenerateRefreshTokenAsync(user, cancellationToken);

            if (accessToken.IsFailure || refreshToken.IsFailure)
            {
                return Error.Failure("Не удалось сгенерировать пару access-token и refresh-token");
            }

            return (accessToken, refreshToken);
        }

        public Result<string> GenerateAccessToken(User user, Role role)
        {
            var claims = new List<Claim>()
            {
                new (CustomClaimTypes.UserId, user.Id.ToString()),
                new (CustomClaimTypes.UserRole, user.Role.ToString())
            };

            return jwtProvider.GenerateAccessToken(claims);
        }

        public async Task<Result> GenerateAndSetTokensAsync(User user, Role role, HttpResponse response, CancellationToken cancellationToken)
        {
            var pair = await GenerateAccessRefreshPairAsync(user, role, cancellationToken);
            if (pair.IsFailure)
            {
                return Error.Failure("Не удалось сгенерировать пару access-token и refresh-token");
            }
            var (accessToken, refreshToken) = pair.Value;
            return Result.Merge(
                SetTokenToHeader(response, accessToken, AuthorizationHeader),
                SetTokenToHeader(response, refreshToken, RefreshTokenHeader)
                );
        }

        public async Task<Result<string>> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>()
            {
                new (CustomClaimTypes.UserId, user.Id.ToString()),
            };
            var token = jwtProvider.GenerateRefreshToken(claims);
            
            if (token.IsFailure)
            {
                return Error.Failure("Не удалось сгенерировать refresh-token");
            }

            var saveResult = await tokenStorage.SetTokenAsync(token, user.Id, cancellationToken)
                .LogIfFailureAsync(
                message: "Не удалось сохранить refresh токен \'{Token}\' для пользователя \'{UserId}\'",
                args: [token, user.Id]
                );
            
            if (saveResult.IsFailure)
            {
                return Error.Failure("Не удалось сохранить refresh-token");
            }
            return token;
        }

        public Result<Role> GetRoleFromAccessToken(HttpRequest request)
        {
            return GetTokenFromHeader(request).Then(token => GetClaimFromToken(
                token, 
                CustomClaimTypes.UserRole, 
                value => Enum.TryParse(value, out Role role) 
                ? Result.Success(role) 
                : Error.Unauthorized("Недействительный токен")
                ));
        }
        private static Result<string> GetTokenFromHeader(HttpRequest request, string header = AuthorizationHeader)
        {
            if (!request.Headers.TryGetValue(header, out var token) || string.IsNullOrEmpty(token))
                return Error.Unauthorized($"Поместите токен в заголовок `{header}`");

            if (header == AuthorizationHeader)
                return token.ToString().Replace(BearerPrefix, "");
            return token.ToString();
        }
        private Result<T> GetClaimFromToken<T>(string token, string claimType, Func<string, Result<T>> parseFunc)
        {
            var validationResult = jwtProvider.ValidateToken(token);

            if (validationResult.IsFailure)
                return validationResult.Errors.First();

            var claim = Result.Try(() => validationResult.Value.First(c => c.Type == claimType))
                .LogIfFailure("Не удалось получить клеймы из токена \'{token}\'", args: token, logLevel: LogLevel.Warning)
                .LogErrorMessages(logLevel: LogLevel.Warning);

            if (claim.IsFailure)
                return Error.Unauthorized("Недействительный токен");

            return parseFunc(((Claim)claim).Value);
        }

        public Result<Guid> GetUserIdFromAccessToken(HttpRequest request, bool isRefresh = false)
        {
            return GetTokenFromHeader(request, (isRefresh?RefreshTokenHeader:AuthorizationHeader)).Then(token => GetClaimFromToken(
                token,
                CustomClaimTypes.UserId,
                value => Guid.TryParse(value, out Guid id)
                ? Result.Success(id)
                : Error.Unauthorized("Недействительный токен")
                ));
        }

        public async Task<Result> RefreshAccessTokenAsync(HttpRequest request, HttpResponse response, User user, CancellationToken cancellationToken)
        {
            var tokenUserIdPair = await GetTokenFromHeader(request, RefreshTokenHeader)
                .ThenAsync(async token => await tokenStorage.GetTokenAsync(token, cancellationToken))
                .LogIfSuccessAsync(
                    message: "Получен refresh токен и id пользователя: {tokenIdPair}",
                    logLevel: LogLevel.Debug
                );
            if (tokenUserIdPair.IsFailure)
                return tokenUserIdPair.Errors.First();

            var (_, userId) = tokenUserIdPair.Value;

            if (userId != user.Id)
                return Error.Unauthorized("Недействительный токен");

            return GetRoleFromAccessToken(request)
                .Then(role => GenerateAccessToken(user, role))
                .Then(accessToken => SetTokenToHeader(response, accessToken, AuthorizationHeader))
                .LogIfSuccess(
                    message: "Access токен для пользователя с id {id} успешно обновлён.",
                    logLevel: LogLevel.Debug,
                    args: user.Id
                );
        }

        public Result ValidateToken(HttpRequest request, string header)
        {
            return GetTokenFromHeader(request, header)
                .Then(jwtProvider.ValidateToken)
                .Then(_ => Result.Success());
        }

        private static Result SetTokenToHeader(HttpResponse response, string token, string header)
        {
            if (header != AuthorizationHeader && header != RefreshTokenHeader)
                return Error.BadRequest("Неверный заголовок");
            if (header == AuthorizationHeader)
                token = BearerPrefix + token;
            response.Headers[header] = token;
            return Result.Success();
        }
    }
}
