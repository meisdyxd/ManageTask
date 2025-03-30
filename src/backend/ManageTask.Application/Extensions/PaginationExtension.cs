using ManageTask.Application.Models.Pagination;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using ResultSharp.Core;
using ResultSharp.Errors;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ManageTask.Domain;

namespace ManageTask.Application.Extensions
{   
    internal static class PaginationExtension
    {
        //private static readonly ILogger logger = LoggerUtil.CreateLogger(nameof(PaginationExtension));
        public static async Task<Result<Paginated<TEntity>>> AsPaginated<TEntity>(this IQueryable<TEntity> query, 
            PaginationParams paginationParams, SortParams? sortParams, CancellationToken cancellationToken)
        {
            try 
            {
                query = query.ApplySorting(sortParams);
                var totalCount = await query.CountAsync(cancellationToken);
                return new Paginated<TEntity>()
                {
                    Items = await query
                        .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                        .Take(paginationParams.PageSize)
                        .ToListAsync(cancellationToken),
                    PaginationParams = paginationParams,
                    HasNextPage = (paginationParams.PageNumber * paginationParams.PageSize) < totalCount,
                    HasPreviewPage = paginationParams.PageNumber > 1,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)paginationParams.PageSize),
                };
            }
            catch (Exception)
            {
                return ResultSharp.Errors.Error.Failure("Ошибка с пагинацией");
            }
        }

        private static IQueryable<T> ApplySorting<T>(this IQueryable<T> data, SortParams? sortParams)
        {
            if (sortParams is not null)
            {
                var expression = $"{sortParams.SortKey} {(sortParams.IsAscending ? "asc" : "desc")}";
                return data.OrderBy(expression);
            }
            else
            {
                return data;
            }
        }

    }
}
