using ManageTask.Domain;
namespace ManageTask.Application.Abstractions.Data
{
    public interface ITaskRepository: IRepository<Domain.Task, Guid>
    {
    }
}
