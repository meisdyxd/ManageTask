using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;

namespace ManageTask.API.Mappers
{
    public static class UserMapper
    {
        public static UserPublic Map(this User user)
            => new(user.Id, user.Name, user.Email, user.Role.ToString());
    }
}
