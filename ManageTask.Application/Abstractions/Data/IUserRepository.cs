using ManageTask.Domain;

namespace ManageTask.Application.Abstractions.Data
{
    public interface IUserRepository: IRepository<User, Guid>
    {
    }
}
