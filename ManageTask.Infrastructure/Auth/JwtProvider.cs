﻿using ManageTask.Application.Abstractions.Auth;
using ManageTask.Infrastructure.ServiceRegistration.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ResultSharp.Core;
using ResultSharp.Errors;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManageTask.Infrastructure.Auth
{
    public class JwtProvider(ILogger<JwtProvider> logger, IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly ILogger<JwtProvider> logger = logger;
        private readonly JwtOptions options = options.Value; 
        public Result<string> GenerateAccessToken(IEnumerable<Claim> claims)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = GetTokenDescriptor(options.Secret, options.AccessTokenExpirationHours, claims.ToArray());
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при генерации JWT токена");
                return Error.Failure("Ошибка генерации Access токена");
            }
        }

        public Result<string> GenerateRefreshToken(IEnumerable<Claim> claims)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = GetTokenDescriptor(
                    options.Secret,
                    options.RefreshTokenExpirationHours,
                    claims.ToArray()
                );

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при генерации JWT токена");
                return Error.Failure("Не удалось сгенерировать refresh токен");
            }
        }

        public Result<IEnumerable<Claim>> ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return Error.Validation("Токен не может быть пустым");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(options.Secret);
                var validationOptions = options.TokenValidation;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = validationOptions.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = validationOptions.ValidateIssuer,
                    ValidateAudience = validationOptions.ValidateAudience,
                    ClockSkew = validationOptions.ClockSkew
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return Result.Success(principal.Claims);
            }
            catch (SecurityTokenExpiredException)
            {
                return Error.Unauthorized("Срок действия токена истек");
            }
            catch (SecurityTokenException ex)
            {
                logger.LogWarning(ex, "Недействительный токен");
                return Error.Unauthorized("Недействительный токен");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при валидации токена");
                return Error.Failure("Не удалось проверить токен");
            }
        }

        private static SecurityTokenDescriptor GetTokenDescriptor(string key, int expirationHours, params Claim[]? claims)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(expirationHours),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };
        }
    }
}
