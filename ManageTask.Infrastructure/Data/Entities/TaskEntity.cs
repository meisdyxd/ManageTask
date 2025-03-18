using ManageTask.Domain;

namespace ManageTask.Infrastructure.Data.Entities
{
    public class TaskEntity: BaseEntity<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StatusTask Status { get; set; } = StatusTask.InPendingUser;
        public bool IsAssigned { get; set; } = false;

        //FK
        public Guid CreatedById { get; set; }
        public Guid? AssignedToId { get; set; }

        //NAV
        public UserEntity CreatedBy { get; set; } = new();
        public UserEntity? AssignedTo { get; set; } = new();
    }
}
