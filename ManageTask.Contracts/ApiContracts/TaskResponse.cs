namespace ManageTask.Contracts.ApiContracts
{
    public record TaskResponse(Guid Id, string Title, string Description, bool Status, Guid CreatedById, Guid AssignedToId, IEnumerable<string> TaskTypes);
}
