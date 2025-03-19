using ManageTask.Application.Abstractions.Auth;
using ManageTask.Infrastructure.ServiceRegistration.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace ManageTask.Infrastructure.Auth
{
    public class PasswordManager(IOptions<PasswordManagerOptions> options) : IPasswordManager
    {
        public Result<string> HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return Error.Validation("Пароль не может быть пустым или состоять только из пробелов");
            }
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public Result<bool> VerifyPassword(string password, string hashedPassword)
        {
            if (password == null)
                return Error.Validation("Пароль не может быть пустым или состоять только из пробелов.");

            if (hashedPassword == null)
                return Error.Validation("Хеш пароля не может быть пустым или состоять только из пробелов.");

            if (string.IsNullOrWhiteSpace(password))
                return Error.Validation("Пароль не может быть пустым или состоять только из пробелов.");

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return Error.Validation("Хеш пароля не может быть пустым или состоять только из пробелов.");

            return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
        }
    }
}
