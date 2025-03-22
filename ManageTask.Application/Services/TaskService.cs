using ManageTask.Application.Abstractions.Data;
using ManageTask.Application.Abstractions.Services;
using ManageTask.Application.Extensions;
using ManageTask.Application.Models.Pagination;
using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;
using ResultSharp.Errors;
using System.Threading;

namespace ManageTask.Application.Services
{
    public class TaskService(ITaskRepository taskRepository, IUserRepository userRepository, IAccountService accountService) : ITaskService
    {
        private readonly ITaskRepository taskRepository = taskRepository;
        private readonly IUserRepository userRepository = userRepository;
        private readonly IAccountService accountService = accountService;
        public async Task<Result<Domain.Task>> AddToPoolAsync(RequestTask requestTask, HttpRequest request, CancellationToken cancellationToken)
        {
            var creator = await accountService.GetCurrentUserAsync(request, cancellationToken);
            if (creator.IsFailure)
            {
                return Error.Unauthorized("Для создания задачи, вам необходимо авторизоваться");
            }
            var task = new Domain.Task(
                Guid.NewGuid(),
                requestTask.Titile,
                requestTask.Description, 
                (StatusTask)requestTask.Status, 
                false, 
                creator.Value.Id, 
                null);
            var savedTask = await taskRepository.AddAsync(task, cancellationToken);
            return savedTask;
        }
        public async Task<Result<Domain.Task>> AddToUserAsync(RequestTask requestTask, Guid assignedId, HttpRequest request, CancellationToken cancellationToken)
        {
            var creator = await accountService.GetCurrentUserAsync(request, cancellationToken);
            if (creator.IsFailure)
            {
                return Error.Unauthorized("Для создания задачи, необходима авторизация");
            }
            var assignedUser = await userRepository.GetAsync(assignedId, cancellationToken);
            if (assignedUser.IsFailure)
            {
                return Error.NotFound("Указанный пользователь не найден");
            }
            var task = new Domain.Task(
                Guid.NewGuid(),
                requestTask.Titile,
                requestTask.Description,
                (StatusTask)requestTask.Status,
                true,
                creator.Value.Id,
                assignedId);
            var savedTask = await taskRepository.AddAsync(task, cancellationToken);
            return savedTask;
        }

        public async Task<Result<Paginated<Domain.Task>>> GetAllAsync(PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var result = taskRepository.GetAll();
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = await result.Value
                .AsPaginatedAsync(paginationParams, sortParams, cancellationToken);
            if (tasks.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            return tasks;
        }

        public async Task<Result<Paginated<Domain.Task>>> GetCurrentAsync(HttpRequest httpRequest, 
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var currentAccount = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken);
            if (currentAccount.IsFailure)
            {
                return Error.Unauthorized("Для получения текущих задач, требуется авторизация");
            }
            var result = taskRepository.GetAll();
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = await result.Value
                .Where(t => t.AssignedToId == currentAccount.Value.Id)
                .AsPaginatedAsync(paginationParams, sortParams, cancellationToken);
            if (tasks.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            return tasks;
        }
        public async Task<Result<Paginated<Domain.Task>>> GetCreatedAsync(HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var currentAccount = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken);
            if (currentAccount.IsFailure)
            {
                return Error.Unauthorized("Для получения текущих задач, требуется авторизация");
            }
            var result = taskRepository.GetAll();
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = await result.Value
                .Where(t => t.CreatedById == currentAccount.Value.Id)
                .AsPaginatedAsync(paginationParams, sortParams, cancellationToken);
            if (tasks.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            return tasks;
        }

        public async Task<Result<Paginated<Domain.Task>>> GetByIdAssignedAsync(Guid assignedId, 
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var result = taskRepository.GetAll();
            if(result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = await result.Value
                .Where(t => t.AssignedToId == assignedId)
                .AsPaginatedAsync(paginationParams, sortParams, cancellationToken);
            if (tasks.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            return tasks;
        }

        public async Task<Result<Domain.Task>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await taskRepository.GetAsync(id, cancellationToken);
        }

        public async Task<Result<Paginated<Domain.Task>>> GetByIdCreatorAsync(Guid creatorId,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var result = taskRepository.GetAll();
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = await result.Value
                .Where(t => t.CreatedById == creatorId)
                .AsPaginatedAsync(paginationParams, sortParams, cancellationToken);
            if (tasks.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            return tasks;
        }

        public async Task<Result<Paginated<Domain.Task>>> GetByStatusCurrentUserAsync(StatusTask statusTask, HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var currentAccount = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken);
            if (currentAccount.IsFailure)
            {
                return Error.Unauthorized("Для получения текущих задач, требуется авторизация");
            }
            var result = taskRepository.GetAll();
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = await result.Value
                .Where(t => t.AssignedToId == currentAccount.Value.Id && t.Status == statusTask)
                .AsPaginatedAsync(paginationParams, sortParams, cancellationToken);
            if (tasks.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            return tasks;
        }

        public async Task<Result<Paginated<Domain.Task>>> GetFromPoolAsync(
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var result = taskRepository.GetAll();
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = await result.Value
                .Where(t => t.Status == StatusTask.InPendingUser && t.IsAssigned == false)
                .AsPaginatedAsync(paginationParams, sortParams, cancellationToken);
            if (tasks.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            return tasks;
        }

        public async Task<Result<Domain.Task>> UpdateAsync(Domain.Task task, CancellationToken cancellationToken)
        {
            if (task.IsAssigned != (task.AssignedToId != null))
            {
                return Error.BadRequest("Ошибка параметров");
            }
            return await taskRepository.UpdateAsync(task, cancellationToken);
        }


    }
}
