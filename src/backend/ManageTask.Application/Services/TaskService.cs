using ManageTask.Application.Abstractions.Data;
using ManageTask.Application.Abstractions.Services;
using ManageTask.Application.Models.Pagination;
using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Core;
using ResultSharp.Errors;
using ManageTask.Application.Extensions;
using ResultSharp.Extensions.FunctionalExtensions.Async;
using ResultSharp.Extensions.FunctionalExtensions.Sync;

namespace ManageTask.Application.Services
{
    public class TaskService(ITaskRepository taskRepository, IUserRepository userRepository, IAccountService accountService) : ITaskService
    {
        private readonly ITaskRepository taskRepository = taskRepository;
        private readonly IUserRepository userRepository = userRepository;
        private readonly IAccountService accountService = accountService;
        public async Task<Result<Domain.TaskM>> AddToPoolAsync(RequestTask requestTask, HttpRequest request, CancellationToken cancellationToken)
        {
            var creator = await accountService.GetCurrentUserAsync(request, cancellationToken);
            if (creator.IsFailure)
            {
                return Error.Unauthorized("Для создания задачи, вам необходимо авторизоваться");
            }
            var task = new Domain.TaskM(
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
        public async Task<Result<Domain.TaskM>> AddToUserAsync(RequestTask requestTask, HttpRequest request, CancellationToken cancellationToken)
        {
            var creator = await accountService.GetCurrentUserAsync(request, cancellationToken);
            if (creator.IsFailure)
            {
                return Error.Unauthorized("Для создания задачи, необходима авторизация");
            }
            var assignedUser = await userRepository.GetAsync(requestTask.AssignedToId.Value, cancellationToken);
            if (assignedUser.IsFailure)
            {
                return Error.NotFound("Указанный пользователь не найден");
            }
            var task = new Domain.TaskM(
                Guid.NewGuid(),
                requestTask.Titile,
                requestTask.Description,
                (StatusTask)requestTask.Status,
                true,
                creator.Value.Id,
                requestTask.AssignedToId);
            var savedTask = await taskRepository.AddAsync(task, cancellationToken);
            return savedTask;
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetAll(PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken)
        {
            var tasks = taskRepository
                .GetAll()
                .Value
                .MatchStatus(status);
            var newTasks = await tasks.AsPaginated(paginationParams, sortParams, cancellationToken);
            return newTasks;
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetCurrentAsync(HttpRequest httpRequest, 
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken)
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
            var tasks = taskRepository
                .GetAll()
                .Value
                .MatchStatus(status)
                .Where(t => t.AssignedToId == currentAccount.Value.Id);
            var newTasks = await tasks.AsPaginated(paginationParams, sortParams, cancellationToken);
            return newTasks;
        }
        public async Task<Result<Paginated<Domain.TaskM>>> GetCreatedAsync(HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken)
        {
            var currentAccount = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken);
            if (currentAccount.IsFailure)
            {
                return Error.Unauthorized("Для получения текущих задач, требуется авторизация");
            }
            var tasks = taskRepository
                .GetAll()
                .Value
                .MatchStatus(status)
                .Where(t => t.CreatedById == currentAccount.Value.Id);
            var newTasks = await tasks.AsPaginated(paginationParams, sortParams, cancellationToken);
            return newTasks;
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetByIdAssigned(Guid assignedId, 
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken)
        {
            var tasks = taskRepository
                .GetAll()
                .Value
                .MatchStatus(status)
                .Where(t => t.AssignedToId == assignedId);
            var newTasks = await tasks.AsPaginated(paginationParams, sortParams, cancellationToken);
            return newTasks;
        }

        public async Task<Result<Domain.TaskM>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await taskRepository.GetAsync(id, cancellationToken);
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetByIdCreator(Guid creatorId,
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken)
        {
            var tasks = taskRepository
                .GetAll()
                .Value
                .MatchStatus(status)
                .Where(t => t.CreatedById == creatorId);
            var newTasks = await tasks.AsPaginated(paginationParams, sortParams, cancellationToken);
            return newTasks;

        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetByStatusCurrentUserAsync(StatusTask statusTask, HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken)
        {
            var currentAccount = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken);
            if (currentAccount.IsFailure)
            {
                return Error.Unauthorized("Для получения текущих задач, требуется авторизация");
            }
            var tasks = taskRepository
                .GetAll()
                .Value
                .MatchStatus(status)
                .Where(t => t.AssignedToId == currentAccount.Value.Id && t.Status == statusTask);
            var newTasks = await tasks.AsPaginated(paginationParams, sortParams, cancellationToken);
            return newTasks;
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetFromPool(
            PaginationParams paginationParams, SortParams? sortParams, int? status, CancellationToken cancellationToken)
        {
            var tasks = taskRepository
                .GetAll()
                .Value
                .Where(t => t.Status == StatusTask.InPendingUser && !t.IsAssigned)
                .MatchStatus(status);
            var newTasks = await tasks.AsPaginated(paginationParams, sortParams, cancellationToken);
            return newTasks;
        }

        public async Task<Result<Domain.TaskM>> UpdateAsync(RequestTask requestTask, Guid taskId, CancellationToken cancellationToken)
        {
            if (requestTask.IsAssigned != (requestTask.AssignedToId is not null))
            {
                return Error.BadRequest("Ошибка параметров");
            }
            var currentTask = await taskRepository.GetAsync(taskId, cancellationToken);
            if (currentTask.IsFailure)
            {
                return Error.BadRequest("Задача для обновления не найдена");
            }
            var task = currentTask.Value;
            var updatedTask = new TaskM
                (
                    task.Id,
                    task.Title,
                    task.Description,
                    task.Status,
                    task.IsAssigned,
                    task.CreatedById,
                    task.AssignedToId
                );
            return await taskRepository.UpdateAsync(updatedTask, cancellationToken);
        }

        public async Task<Result<Domain.TaskM>> ChangeStatusAsync(Guid taskId, HttpRequest request, StatusTask statusTask, CancellationToken cancellationToken)
        {
            var creator = await accountService.GetCurrentUserAsync(request, cancellationToken);
            if (creator.IsFailure)
            {
                return Error.Unauthorized("Для создания задачи, вам необходимо авторизоваться");
            }
            var task = await taskRepository.GetAsync(taskId, cancellationToken);
            if (task.IsFailure)
            {
                return Error.Failure("Ошибка получения задачи");
            }
            if (creator.Value.Role == Role.User && task.Value.AssignedToId != creator.Value.Id)
            {
                return Error.Forbidden("Доступ ограничен");
            }
            task.Value.Status = statusTask;
            var cancelledTask = await taskRepository.UpdateAsync(task, cancellationToken);
            return cancelledTask;
        }
        public async Task<Result<Domain.TaskM>> TakeAsync(Guid taskId, HttpRequest request, CancellationToken cancellationToken)
        {
            var currentUser = await accountService.GetCurrentUserAsync(request, cancellationToken);
            if (currentUser.IsFailure)
            {
                return Error.Unauthorized("Для создания задачи, вам необходимо авторизоваться");
            }
            var task = await taskRepository.GetAsync(taskId, cancellationToken);
            if (task.IsFailure)
            {
                return Error.Failure("Ошибка получения задачи");
            }
            if (task.Value.IsAssigned && task.Value.Status != StatusTask.InPendingUser)
            {
                return Error.Forbidden("Доступ ограничен");
            }
            task.Value.Status = StatusTask.InProcess;
            task.Value.IsAssigned = true;
            task.Value.AssignedToId = currentUser.Value.Id;
            var cancelledTask = await taskRepository.UpdateAsync(task, cancellationToken);
            return cancelledTask;
        }
        

    }
}
