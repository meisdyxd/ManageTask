namespace ManageTask.Application.Models.Pagination
{
    public record PaginationParams
    {
        public const int MaxPageSize = 50;
        public const int StartPage = 1;
        public int PageNumber { get; init; } = StartPage;
        public int PageSize { get; init; } = MaxPageSize;
    }
}
