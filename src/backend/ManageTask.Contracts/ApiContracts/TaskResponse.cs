using ManageTask.Domain;

namespace ManageTask.Contracts.ApiContracts
{
    public record TaskResponse(Guid Id, string Title, string Description, StatusTask Status, bool IsAssigned, Guid CreatedById, Guid? AssignedToId);
}
