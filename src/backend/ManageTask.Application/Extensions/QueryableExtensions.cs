using ManageTask.Domain;

namespace ManageTask.Application.Extensions
{
    internal static class QueryableExtensions
    {
        public static IQueryable<TaskM> MatchStatus(this IQueryable<TaskM> query, int? status)
        {
            if (status is null)
            {
                return query;
            }
            return query.Where(t => t.Status == (StatusTask)status);
        }
        public static IQueryable<User> MatchName(this IQueryable<User> query, string? name)
        {
            if (name is null)
            {
                return query;
            }
            return query.Where(t => t.Name.ToLower().Contains(name));
        }
    }
}
