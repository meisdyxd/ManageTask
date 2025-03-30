using ManageTask.Domain;
using ResultSharp.Core;

namespace ManageTask.Application.Abstractions.Data
{
    public interface IUserRepository: IRepository<User, Guid>
    {
        Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<Result<User>> UpdateAsync(User user, CancellationToken cancellationToken);
        Task<Result<User>> GetAdminAsync(Guid id, CancellationToken cancellationToken);
    }
}
