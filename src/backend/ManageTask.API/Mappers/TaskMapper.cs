using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;
using ManageTask.Infrastructure.Data.Entities;

namespace ManageTask.API.Mappers
{
    public static class TaskMapper
    {
        public static TaskResponse Map(this Domain.TaskM task)
            => new(task.Id, task.Title, task.Description, task.Status, task.IsAssigned, task.CreatedById, task.AssignedToId);
        public static TaskResponse Map(this TaskEntity task)
            => new(task.Id, task.Title, task.Description, task.Status, task.IsAssigned, task.CreatedById, task.AssignedToId);
    }
}
//public record TaskResponse(Guid Id, string Title, string Description, bool Status, Guid CreatedById, Guid AssignedToId, IEnumerable<string> TaskTypes);