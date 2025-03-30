using ManageTask.Domain;
using ManageTask.Infrastructure.Data.Entities;

namespace ManageTask.Infrastructure.Data.Mappers
{
    public static class UserEntityMapper
    {
        public static User Map(this UserEntity entity)
            => new User 
            { 
                Id = entity.Id, 
                Name = entity.Name, 
                Email = entity.Email,
                Password = entity.Password,
                Role = entity.Role
            };
        public static UserEntity Map(this User user)
            => new()
            {
                Id = user.Id,
                Name = user.Name,
                Password = user.Password,
                Email = user.Email,
                Role = user.Role
            };
    }
}
