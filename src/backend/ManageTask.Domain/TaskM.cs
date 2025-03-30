using ManageTask.Domain.Abstractions;

namespace ManageTask.Domain
{
    public class TaskM
        :EntityObject<Guid>
    {
        public TaskM(Guid id, string title, string description, StatusTask status, bool isAssigned, Guid createdById, Guid? assignedToId): base(id)
        {
            Id = id;
            Title = title;
            Description = description;
            Status = status;
            IsAssigned = isAssigned;
            CreatedById = createdById;
            AssignedToId = assignedToId;
        }
        public TaskM() : base(Guid.NewGuid()) // Или другой способ инициализации Id
        {
        }
        public string Title { get;  set; }
        public string Description { get;  set; }
        public StatusTask Status { get;  set; }
        public bool IsAssigned { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? AssignedToId { get; set; }
        public void AssignToUser(Guid userId)
        {
            if (!IsAssigned)
            {
                AssignedToId = userId;
                IsAssigned = true;
                Status = StatusTask.InProcess;
            }
        }
        public TaskM Create(Guid id, string title, string description, StatusTask status, bool isAssigned, Guid createdById, Guid? assignedToId)
        {
            return new TaskM(id, title, description, status, isAssigned, createdById, assignedToId);
        }
    }
}
