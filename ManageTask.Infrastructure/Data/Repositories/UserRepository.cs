using ManageTask.Application.Abstractions.Data;
using ManageTask.Domain;
using ResultSharp.Core;

namespace ManageTask.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<Result<User>> AddAsync(User entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Result<IQueryable<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Result<User>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<User>> UpdateAsync(User entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
