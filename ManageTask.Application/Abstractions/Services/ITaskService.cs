using ManageTask.Application.Models.Pagination;
using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;

namespace ManageTask.Application.Abstractions.Services
{
    public interface ITaskService
    {
        Task<Result<Domain.Task>> UpdateAsync(Domain.Task task, CancellationToken cancellationToken);
        Task<Result<Domain.Task>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Result<Domain.Task>> AddToPoolAsync(RequestTask task, HttpRequest request, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.Task>>> GetAllAsync(PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.Task>>> GetCurrentAsync(HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.Task>>> GetCreatedAsync(HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.Task>>> GetByIdAssignedAsync(Guid assignedId,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.Task>>> GetByIdCreatorAsync(Guid creatorId,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.Task>>> GetByStatusCurrentUserAsync(StatusTask statusTask, HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.Task>>> GetFromPoolAsync(
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken);
        Task<Result<Domain.Task>> AddToUserAsync(RequestTask task, Guid assignedId, HttpRequest request, CancellationToken cancellationToken);
    }
}
