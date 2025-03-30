using ManageTask.Domain;

namespace ManageTask.Infrastructure.Data.Entities
{
    public class UserEntity: BaseEntity<Guid>
    {
        public Guid Id { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = new();
        
        //NAV
        public List<TaskEntity> CreatedTasks { get; set; } = new();
        public List<TaskEntity> AssignedTasks { get; set; } = new();
    }
}
