using ManageTask.Application.Abstractions.Auth;
using ManageTask.Application.Abstractions.Data;
using ManageTask.Application.Abstractions.Services;
using ManageTask.Application.Extensions;
using ManageTask.Application.Models.Pagination;
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

        public async Task<Result<Paginated<UserPublic>>> GetAll(PaginationParams paginationParams, SortParams? sortParams, string? name, CancellationToken cancellationToken)
        {
            var users = userRepository
                .GetAll()
                .Value
                .MatchName(name)
                .Select(u => new UserPublic { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role.ToString() });
            var newUsers = await users.AsPaginated(paginationParams, sortParams, cancellationToken);
            return newUsers;
        }

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

            var newUser = new User 
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                Password = hash,
                Role = Role.User
            };

            var savedUser = await userRepository.AddAsync(newUser, cancellationToken);
            await savedUser.ThenAsync(
                user => authService.GenerateAndSetTokensAsync(
                    user,
                    Role.User,
                    response,
                    cancellationToken
                ));

            return savedUser;
        }

        public async Task<Result<User>> UpdateAsync(Guid id, RegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            return await userRepository.UpdateAsync(new
                (
                    id, 
                    registerRequest.Name, 
                    registerRequest.Password, 
                    registerRequest.Email, 
                    (Role)registerRequest.Role
                ), cancellationToken);
        }
        public async Task<Result<User>> GetAdminAsync(Guid id, CancellationToken cancellationToken)
        {
            return await userRepository.GetAdminAsync(id, cancellationToken);
        }
    }
}
