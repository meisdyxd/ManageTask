namespace ManageTask.Application.Models.Pagination
{
    public record SortParams
    {
        public required string SortKey { get; init; }
        public bool IsAscending { get; init; } = true;
    }
}
