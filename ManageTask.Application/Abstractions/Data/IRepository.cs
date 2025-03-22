using ManageTask.Application.Models.Pagination;
using ResultSharp.Core;
using System.Security.Cryptography;

namespace ManageTask.Application.Abstractions.Data
{
    public interface IRepository<TEntity, TId>
    {
        Task<Result<TEntity>> GetAsync(TId id, CancellationToken cancellationToken);
        Task<Result<IQueryable<TEntity>>> GetAllAsync(PaginationParams paginationParams, SortParams? sortParams,
            CancellationToken cancellationToken);
        Task<Result<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
