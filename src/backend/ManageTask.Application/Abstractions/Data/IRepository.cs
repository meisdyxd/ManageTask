using ManageTask.Application.Models.Pagination;
using ResultSharp.Core;
using System.Security.Cryptography;

namespace ManageTask.Application.Abstractions.Data
{
    public interface IRepository<TEntity, TId>
    {
        Task<Result<TEntity>> GetAsync(TId id, CancellationToken cancellationToken);
        Result<IQueryable<TEntity>> GetAll();
        Task<Result<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
