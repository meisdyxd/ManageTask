using ManageTask.Domain;
using ResultSharp.Core;

namespace ManageTask.Application.Abstractions.Data
{
    public interface IUserRepository: IRepository<User, Guid>
    {
        Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
