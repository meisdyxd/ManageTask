namespace ManageTask.Infrastructure.Data.Entities
{
    public class TaskEntity: BaseEntity<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StatusTaskEntity Status { get; set; } = new();
        public Guid CreatedById { get; set; }
        public Guid AssignedToId { get; set; }
        public TypeTaskEntity TypeTask { get; set; } = new();

        public UserEntity CreatedBy { get; set; } = new();
        public UserEntity AssignedTo { get; set; } = new();
    }
}
