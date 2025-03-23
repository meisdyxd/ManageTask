using ManageTask.Application.Abstractions.Data;
using ManageTask.Application.Models.Pagination;
using ManageTask.Domain;
using ManageTask.Infrastructure.Data.Contexts;
using ManageTask.Infrastructure.Data.Entities;
using ManageTask.Infrastructure.Data.Mappers;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Extensions.FunctionalExtensions.Sync;
using ManageTask.Application.Extensions;
using ResultSharp.Extensions.FunctionalExtensions.Async;

namespace ManageTask.Infrastructure.Data.Repositories
{
    public class TaskRepository(DataContext context, ILogger<TaskRepository> logger) : ITaskRepository
    {
        private readonly ILogger<TaskRepository> logger = logger;
        private readonly DataContext context = context;
        public async Task<Result<Domain.TaskM>> AddAsync(Domain.TaskM task, CancellationToken cancellationToken)
        {
            logger.LogInformation("Добавление задачи с ID: {TaskId}", task.Id);

            var entity = GetTaskEntity(task);
            await context.Tasks.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync();

            logger.LogInformation("Задача с ID: {TaskId} добавлена", entity.Id);
            return entity.Map();
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("Удаление задачи с ID: {TaskId}", id);

            var entity = await context.Tasks.FindAsync(keyValues: [id], cancellationToken: cancellationToken);
            if(entity is null)
            {
                logger.LogWarning("Задача с ID: {TaskId} не найдена для удаления", id);
                return Error.NotFound($"Задача с ID: {id} не найдена");
            }
            context.Tasks.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Задача с ID: {TaskId} успешно удалена", id);
            return Result.Success();
        }

        public async Task<Result<IQueryable<Domain.TaskM>>> GetAllAsync(PaginationParams paginationParams, SortParams? sortParams, 
            CancellationToken cancellationToken)
        {
            logger.LogInformation($"Получение задач");
            var paginationResult = await context.Tasks.AsQueryable().AsPaginatedAsync(paginationParams, sortParams, cancellationToken);
            return Result<IQueryable<Domain.TaskM>>.Success(paginationResult.Value.Select(t => t.Map()).AsQueryable());
        }

        public async Task<Result<Domain.TaskM>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("Получение задачи с ID: {TaskId}", id);

            var entity = await context.Tasks.FindAsync(keyValues: [id], cancellationToken: cancellationToken);
            if (entity is null)
            {
                logger.LogWarning("Задача с ID: {TaskId}, для получения не найдена", id);
                return Error.NotFound($"Задача с ID: {id} не найдена");
            }

            logger.LogInformation("Задача с ID: {TaskId} получена", id);
            return entity.Map();
        }

        public async Task<Result<Domain.TaskM>> UpdateAsync(Domain.TaskM task, CancellationToken cancellationToken)
        {
            logger.LogInformation("Обновление задачи с ID: {TaskId}", task.Id);

            var entity = GetTaskEntity(task);
            context.Tasks.Update(entity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Задача с ID: {TaskId} успешно обновлена", entity.Id);
            return entity.Map();
        }

        public TaskEntity GetTaskEntity(Domain.TaskM task)
        {
            return task.Map();
        }
    }
}
