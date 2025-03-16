namespace ManageTask.Infrastructure.Data.Entities
{
    public class UserEntity: BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public RoleEntity Role { get; set; } = new();
        
        public List<TaskEntity> Tasks { get; set; } = new();
    }
}
