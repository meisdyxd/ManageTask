using ManageTask.Domain;

namespace ManageTask.Contracts.ApiContracts
{
    public enum PublicTypeTask
    {
        Assigned = TypeTask.Assigned,
        Pool = TypeTask.Pool
    }
    public enum PublicStatusTask
    {
        OnHold = StatusTask.OnHold,
        InProgress = StatusTask.InProgress,
        Success = StatusTask.Success,
        Canceled = StatusTask.Canceled
    }
    public record RequestTask(string Titile, string Description, PublicStatusTask Status, Guid CreatedById, Guid AssignedToId, PublicTypeTask TypeTask);
}
