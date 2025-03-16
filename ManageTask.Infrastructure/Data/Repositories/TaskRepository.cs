using ManageTask.Application.Abstractions.Data;
using ResultSharp.Core;

namespace ManageTask.Infrastructure.Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        public Task<Result<Domain.Task>> AddAsync(Domain.Task entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Result<IQueryable<Domain.Task>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Result<Domain.Task>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Domain.Task>> UpdateAsync(Domain.Task entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
