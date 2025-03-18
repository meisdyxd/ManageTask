using ManageTask.Domain.Abstractions;

namespace ManageTask.Domain
{
    public class Task(Guid id, string title, string description, StatusTask status, bool isAssigned, Guid createdById, Guid? assignedToId)
        :EntityObject<Guid>(id)
    {
        public string Title { get; private set; } = title;
        public string Description { get; private set; } = description;
        public StatusTask Status { get; private set; } = status;
        public bool IsAssigned { get; private set; } = isAssigned;
        public Guid CreatedById { get; private set; } = createdById;
        public Guid? AssignedToId { get; private set; } = assignedToId;
        public void AssignToUser(Guid userId)
        {
            if (!IsAssigned)
            {
                AssignedToId = userId;
                IsAssigned = true;
                Status = StatusTask.InProcess;
            }
        }
    }
}
