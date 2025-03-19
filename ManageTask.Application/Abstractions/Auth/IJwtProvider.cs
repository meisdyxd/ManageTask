using Microsoft.IdentityModel.Tokens;
using ResultSharp.Core;
using System.Security.Claims;

namespace ManageTask.Application.Abstractions.Auth
{
    public interface IJwtProvider
    {
        public Result<string> GenerateAccessToken(IEnumerable<Claim> claims);
        Result<IEnumerable<Claim>> ValidateToken(string token);
        Result<string> GenerateRefreshToken();
    }
}
