using ManageTask.Application.Abstractions.Data;
using ManageTask.Application.Abstractions.Services;
using ManageTask.Application.Models.Pagination;
using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Core;
using ResultSharp.Errors;
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
        public async Task<Result<Domain.TaskM>> AddToUserAsync(RequestTask requestTask, Guid assignedId, HttpRequest request, CancellationToken cancellationToken)
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
            var task = new Domain.TaskM(
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

        public async Task<Result<Paginated<Domain.TaskM>>> GetAll(PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var result = await taskRepository.GetAllAsync(paginationParams, sortParams, cancellationToken);
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = result.Value.ToList();
            var res = new Paginated<Domain.TaskM>()
            {
                Items = tasks,
                PaginationParams = paginationParams,
                HasNextPage = true,
                HasPreviewPage = true,
                TotalPages = tasks.Count,
            };
            return res;
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetCurrentAsync(HttpRequest httpRequest, 
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var currentAccount = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken);
            if (currentAccount.IsFailure)
            {
                return Error.Unauthorized("Для получения текущих задач, требуется авторизация");
            }
            var result = await taskRepository.GetAllAsync(paginationParams, sortParams, cancellationToken);
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = result.Value
                .Where(t => t.AssignedToId == currentAccount.Value.Id).ToList();
            var res = new Paginated<Domain.TaskM>()
            {
                Items = tasks,
                PaginationParams = paginationParams,
                HasNextPage = true,
                HasPreviewPage = true,
                TotalPages = tasks.Count,
            };
            return res;
        }
        public async Task<Result<Paginated<Domain.TaskM>>> GetCreatedAsync(HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var currentAccount = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken);
            if (currentAccount.IsFailure)
            {
                return Error.Unauthorized("Для получения текущих задач, требуется авторизация");
            }
            var result = await taskRepository.GetAllAsync(paginationParams, sortParams, cancellationToken);
            var tasks = result.Value
                .Where(t => t.CreatedById == currentAccount.Value.Id).ToList();
            var res = new Paginated<Domain.TaskM>()
            {
                Items = tasks,
                PaginationParams = paginationParams,
                HasNextPage = true,
                HasPreviewPage = true,
                TotalPages = tasks.Count,
            };
            return res;
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetByIdAssignedAsync(Guid assignedId, 
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var result = await taskRepository.GetAllAsync(paginationParams, sortParams, cancellationToken);
            var tasks = result.Value
                .Where(t => t.AssignedToId == assignedId).ToList();
            var res = new Paginated<Domain.TaskM>()
            {
                Items = tasks,
                PaginationParams = paginationParams,
                HasNextPage = true,
                HasPreviewPage = true,
                TotalPages = tasks.Count,
            };
            return res;
        }

        public async Task<Result<Domain.TaskM>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await taskRepository.GetAsync(id, cancellationToken);
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetByIdCreatorAsync(Guid creatorId,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var result = await taskRepository.GetAllAsync(paginationParams, sortParams, cancellationToken);
            var tasks = result.Value
                .Where(t => t.CreatedById == creatorId).ToList();
            var res = new Paginated<Domain.TaskM>()
            {
                Items = tasks,
                PaginationParams = paginationParams,
                HasNextPage = true,
                HasPreviewPage = true,
                TotalPages = tasks.Count,
            };
            return res;
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetByStatusCurrentUserAsync(StatusTask statusTask, HttpRequest httpRequest,
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var currentAccount = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken);
            if (currentAccount.IsFailure)
            {
                return Error.Unauthorized("Для получения текущих задач, требуется авторизация");
            }
            var result = await taskRepository.GetAllAsync(paginationParams, sortParams, cancellationToken);
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = result.Value
                .Where(t => t.AssignedToId == currentAccount.Value.Id && t.Status == statusTask).ToList();
            var res = new Paginated<Domain.TaskM>()
            {
                Items = tasks,
                PaginationParams = paginationParams,
                HasNextPage = true,
                HasPreviewPage = true,
                TotalPages = tasks.Count,
            };
            return res;
        }

        public async Task<Result<Paginated<Domain.TaskM>>> GetFromPoolAsync(
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            var result = await taskRepository.GetAllAsync(paginationParams, sortParams, cancellationToken);
            if (result.IsFailure)
            {
                Error.Failure("Ошибка получения данных");
            }
            var tasks = result.Value
                .Where(t => t.Status == StatusTask.InPendingUser && t.IsAssigned == false).ToList();
            var res = new Paginated<Domain.TaskM>()
            {
                Items = tasks,
                PaginationParams = paginationParams,
                HasNextPage = true,
                HasPreviewPage = true,
                TotalPages = tasks.Count,
            };
            return res;
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

        public async Task<Result<Domain.TaskM>> CancelAsync(Guid taskId, HttpRequest request, CancellationToken cancellationToken)
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
            task.Value.Status = StatusTask.Cancelled;
            var cancelledTask = await taskRepository.UpdateAsync(task, cancellationToken);
            return cancelledTask;
        }


    }
}
