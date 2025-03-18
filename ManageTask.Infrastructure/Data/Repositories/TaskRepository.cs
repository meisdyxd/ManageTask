using ManageTask.Application.Abstractions.Data;
using ManageTask.Domain;
using ManageTask.Infrastructure.Data.Contexts;
using ManageTask.Infrastructure.Data.Entities;
using ManageTask.Infrastructure.Data.Mappers;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Extensions.FunctionalExtensions.Sync;

namespace ManageTask.Infrastructure.Data.Repositories
{
    public class TaskRepository(DataContext context, ILogger<TaskRepository> logger) : ITaskRepository
    {
        private readonly ILogger<TaskRepository> logger = logger;
        private readonly DataContext context = context;
        public async Task<Result<Domain.Task>> AddAsync(Domain.Task task, CancellationToken cancellationToken)
        {
            logger.LogInformation("Добавление задачи с ID: {TaskId}", task.Id);

            var entity = GetTaskEntity(task);
            await context.AddAsync(entity, cancellationToken);
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

        public Result<IQueryable<Domain.Task>> GetAll()
        {
            logger.LogInformation($"Получение всех задач");
            return Result<IQueryable<Domain.Task>>.Success(context.Tasks.Select(u => u.Map()).AsQueryable());
        }

        public async Task<Result<Domain.Task>> GetAsync(Guid id, CancellationToken cancellationToken)
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
        public Result<IQueryable<Domain.Task>> GetByIdCreatorAsync(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("Получение задач с ID создателя: {UserId}", id);

            var tasks = context.Tasks.Where(t => t.CreatedById == id).Select(t => t.Map()).AsQueryable();

            logger.LogInformation("Задачи с ID создателя: {UserId} получены", id);
            return Result<IQueryable<Domain.Task>>.Success(tasks);
        }
        public Result<IQueryable<Domain.Task>> GetByIdAssignedAsync(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("Получение задач с ID назначенного пользователя: {UserId}", id);

            var tasks = context.Tasks.Where(t => t.AssignedToId == id).Select(t => t.Map()).AsQueryable();

            logger.LogInformation("Задачи с ID назначеноого пользователя: {UserId} получены", id);
            return Result<IQueryable<Domain.Task>>.Success(tasks);
        }

        public async Task<Result<Domain.Task>> UpdateAsync(Domain.Task task, CancellationToken cancellationToken)
        {
            logger.LogInformation("Обновление задачи с ID: {TaskId}", task.Id);

            var entity = task.Map();
            context.Tasks.Update(entity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Задача с ID: {TaskId} успешно обновлена", entity.Id);
            return entity.Map();
        }

        public TaskEntity GetTaskEntity(Domain.Task task)
        {
            return task.Map();
        } 
    }
}
