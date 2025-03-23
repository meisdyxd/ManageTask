using ManageTask.Application.Abstractions.Auth;
using ManageTask.Application.Abstractions.Data;
using ManageTask.Application.Abstractions.Services;
using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Extensions.FunctionalExtensions.Async;
using ResultSharp.Logging;

namespace ManageTask.Application.Services
{
    public class AccountService(IUserRepository userRepository, IAuthService authService, IPasswordManager passwordManager) : IAccountService
    {
        private readonly IUserRepository userRepository = userRepository;
        private readonly IAuthService authService = authService;
        private readonly IPasswordManager passwordManager = passwordManager;

        public async Task<Result<User>> GetCurrentUserAsync(HttpRequest request, CancellationToken cancellationToken, bool isRefresh = false)
        {
            return await authService.GetUserIdFromAccessToken(request, isRefresh)
                .ThenAsync(id => userRepository.GetAsync(id, cancellationToken))
                .LogIfFailureAsync("Ошибка при получении текущего аккаунта");
        }

        public async Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await userRepository.GetByEmailAsync(email, cancellationToken);
        }

        public async Task<Result<User>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await userRepository.GetAsync(id, cancellationToken);
        }

        public async Task<Result> LoginAsync(LoginRequest request, HttpResponse response, CancellationToken cancellationToken)
        {
            return await userRepository.GetByEmailAsync(request.Email, cancellationToken)
                .EnsureAsync(user => passwordManager.VerifyPassword(request.Password, user.Password), Error.Unauthorized("Неверный email или пароль"))
                .ThenAsync(user => authService.GenerateAndSetTokensAsync(user, user.Role, response, cancellationToken));
        }

        public async Task<Result> LogoutAsync(HttpRequest request, HttpResponse response, CancellationToken cancellationToken)
        {
            return await authService.ClearTokensAsync(request, response, cancellationToken);
        }

        public async Task<Result> RefreshToken(HttpRequest request, HttpResponse response, CancellationToken cancellationToken)
        {
            return await GetCurrentUserAsync(request, cancellationToken, true)
                .ThenAsync(user => authService.RefreshAccessTokenAsync(request, response, user, cancellationToken));
        }

        public async Task<Result<User>> RegisterAsync(RegisterRequest request, HttpResponse response, CancellationToken cancellationToken)
        {
            var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser.IsSuccess)
                return Error.Conflict("Пользователь с таким email уже существует");


            var hash = passwordManager.HashPassword(request.Password);
            if (hash.IsFailure)
                return hash.Errors.First();

            var newUser = new User(
                Guid.NewGuid(),
                request.Username,
                hash,
                request.Email,
                (Role)request.Role
            );

            var savedUser = await userRepository.AddAsync(newUser, cancellationToken);
            await savedUser.ThenAsync(
                user => authService.GenerateAndSetTokensAsync(
                    user,
                    (Role)request.Role,
                    response,
                    cancellationToken
                ));

            return savedUser;
        }
    }
}
