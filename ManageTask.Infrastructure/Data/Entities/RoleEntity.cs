namespace ManageTask.Infrastructure.Data.Entities
{
    public class RoleEntity
    {
        public string Name { get; set; } = string.Empty;
        public List<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}
