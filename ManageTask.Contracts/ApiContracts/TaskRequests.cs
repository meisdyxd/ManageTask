using ManageTask.Domain;

namespace ManageTask.Contracts.ApiContracts
{
    public enum PublicStatusTask
    {
        InPendingUser = StatusTask.InPendingUser,
        InProcess = StatusTask.InProcess,
        OnReview = StatusTask.OnReview,
        Success = StatusTask.Success,
        Cancelled = StatusTask.Cancelled
    }
    public record RequestTask(string Titile, string Description, PublicStatusTask Status, bool IsAssigned, Guid CreatedById, Guid AssignedToId);
}
