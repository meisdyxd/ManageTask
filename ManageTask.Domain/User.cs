using ManageTask.Domain.Abstractions;

namespace ManageTask.Domain
{
    public class User(Guid id, string username, string password, string email, Role role):
        EntityObject<Guid>(id)
    {
        public string Username { get; private set; } = username;
        public string Password { get; private set; } = password;
        public string Email { get; private set; } = email;
        public Role Roles { get; private set; } = role;
    }
}
