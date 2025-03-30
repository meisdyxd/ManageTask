using ManageTask.Infrastructure.Data.Entities;

namespace ManageTask.Infrastructure.Data.Mappers
{
    public static class TaskEntityMapper
    {
        public static Domain.TaskM Map(this TaskEntity entity)
            => new
            (
                entity.Id,
                entity.Title,
                entity.Description,
                entity.Status,
                entity.IsAssigned,
                entity.CreatedById,
                entity.AssignedToId
            );
        public static TaskEntity Map(this Domain.TaskM task)
            => new()
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                IsAssigned = task.IsAssigned,
                CreatedById = task.CreatedById,
                AssignedToId = task.AssignedToId
            };
    }
}
