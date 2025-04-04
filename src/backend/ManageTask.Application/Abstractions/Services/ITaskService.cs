﻿using ManageTask.Application.Models.Pagination;
using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;

namespace ManageTask.Application.Abstractions.Services
{
    public interface ITaskService
    {
        Task<Result<Domain.TaskM>> UpdateAsync(RequestTask requestTask, Guid taskId, CancellationToken cancellationToken);
        Task<Result<Domain.TaskM>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Result<Domain.TaskM>> AddToPoolAsync(RequestTask task, HttpRequest request, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.TaskM>>> GetAll(PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.TaskM>>> GetCurrentAsync(HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.TaskM>>> GetCreatedAsync(HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.TaskM>>> GetByIdAssigned(Guid assignedId,
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.TaskM>>> GetByIdCreator(Guid creatorId,
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.TaskM>>> GetByStatusCurrentUserAsync(StatusTask statusTask, HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken);
        Task<Result<Paginated<Domain.TaskM>>> GetFromPool(
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken);
        Task<Result<Domain.TaskM>> AddToUserAsync(RequestTask task, HttpRequest request, CancellationToken cancellationToken);
        Task<Result<Domain.TaskM>> ChangeStatusAsync(Guid taskId, HttpRequest request, StatusTask statusTask, CancellationToken cancellationToken);
        Task<Result<Domain.TaskM>> TakeAsync(Guid taskId, HttpRequest request, CancellationToken cancellationToken);
    }
}
