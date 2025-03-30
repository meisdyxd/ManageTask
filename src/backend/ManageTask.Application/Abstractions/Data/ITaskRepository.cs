using ManageTask.Domain;
using ResultSharp.Core;
namespace ManageTask.Application.Abstractions.Data
{
    public interface ITaskRepository: IRepository<Domain.TaskM, Guid>
    {
        Task<Result<TaskM>> UpdateAsync(TaskM task, CancellationToken cancellationToken);
    }
}
