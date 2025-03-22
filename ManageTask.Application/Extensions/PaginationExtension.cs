using ManageTask.Application.Models.Pagination;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using ResultSharp.Core;
using ResultSharp.Errors;
using Microsoft.EntityFrameworkCore;

namespace ManageTask.Application.Extensions
{   
    internal static class PaginationExtension
    {
        //private static readonly ILogger logger = LoggerUtil.CreateLogger(nameof(PaginationExtension));
        public static async Task<Result<Paginated<T>>> AsPaginatedAsync<T>(this IQueryable<T> query, 
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            try 
            {
                query = query.ApplySorting(sortParams);
                var result = await query
                    .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToListAsync(cancellationToken);
                var totalItems = result.LongCount();
                var totalPages = (int)Math.Ceiling((double)(totalItems/paginationParams.PageSize));

                return new Paginated<T>()
                {
                    Items = result,
                    PaginationParams = paginationParams,
                    TotalPages = totalPages,
                    HasPreviewPage = paginationParams.PageNumber > PaginationParams.StartPage,
                    HasNextPage = paginationParams.PageNumber < totalPages
                };
            }
            catch (Exception)
            {
                return Error.Failure("Ошибка с пагинацией");
            }
        }

        public static Result<Paginated<T>> AsPaginated<T>(this IQueryable<T> query,
            PaginationParams paginationParams, SortParams? sortParams)
        {
            try
            {
                query = query.ApplySorting(sortParams);
                var result = query
                    .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToList()
                    .AsReadOnly();
                var totalItems = result.LongCount();
                var totalPages = (int)Math.Ceiling((double)(totalItems / paginationParams.PageSize));

                return new Paginated<T>()
                {
                    Items = result,
                    PaginationParams = paginationParams,
                    TotalPages = totalPages,
                    HasPreviewPage = paginationParams.PageNumber > PaginationParams.StartPage,
                    HasNextPage = paginationParams.PageNumber < totalPages
                };
            }
            catch (Exception)
            {
                return Error.Failure("Ошибка с пагинацией");
            }
        }

        private static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, SortParams? sortParams)
        {
            if(sortParams is not null)
            {
                var expression = $"{sortParams.SortKey} {(sortParams.IsAscending ? "asc" : "desc")}";
                return query.OrderBy(expression);
            }
            else
            {
                return query.Order();
            }
        }
    }
}
