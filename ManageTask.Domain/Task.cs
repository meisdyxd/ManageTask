using ManageTask.Domain.Abstractions;

namespace ManageTask.Domain
{
    public class Task(Guid id, string title, string description, StatusTask status, Guid createdById, Guid? assignedToId, TypeTask type)
        :EntityObject<Guid>(id)
    {
        public string Title { get; private set; } = title;
        public string Description { get; private set; } = description;
        public StatusTask Status { get; private set; } = status;
        public Guid? CreatedById { get; private set; } = createdById;
        public Guid? AssignedToId { get; private set; } = assignedToId;
        public TypeTask Types { get; private set; } = type;
    }
}
