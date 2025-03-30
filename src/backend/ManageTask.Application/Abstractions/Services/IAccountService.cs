using ManageTask.Application.Models.Pagination;
using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;

namespace ManageTask.Application.Abstractions.Services
{
    public interface IAccountService
    {
        Task<Result<User>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Result<User>> GetCurrentUserAsync(HttpRequest request, CancellationToken cancellationToken, bool isRefresh = false);
        Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task<Result<User>> RegisterAsync(RegisterRequest request, HttpResponse response, CancellationToken cancellationToken);
        Task<Result> LoginAsync(LoginRequest request, HttpResponse response, CancellationToken cancellationToken);
        Task<Result> LogoutAsync(HttpRequest request, HttpResponse response, CancellationToken cancellationToken);
        Task<Result> RefreshToken(HttpRequest request, HttpResponse response, CancellationToken cancellationToken);
        Task<Result<Paginated<UserPublic>>> GetAll(PaginationParams paginationParams, SortParams? sortParams, string? name, CancellationToken cancellationToken);
        Task<Result<User>> UpdateAsync(Guid id, RegisterRequest registerRequest, CancellationToken cancellationToken);
    }
}
