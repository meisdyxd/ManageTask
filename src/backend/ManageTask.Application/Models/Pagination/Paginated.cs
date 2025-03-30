namespace ManageTask.Application.Models.Pagination
{
    public record Paginated<T>
    {
        public required PaginationParams PaginationParams { get; init; }
        public bool HasPreviewPage { get; init; }
        public bool HasNextPage { get; init; }
        public int TotalPages { get; init; }
        public required IReadOnlyCollection<T> Items { get; init; }
    }
}
