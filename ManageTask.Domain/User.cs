using ManageTask.Domain.Abstractions;

namespace ManageTask.Domain
{
    public class User(Guid id, string Name, string password, string email, Role role):
        EntityObject<Guid>(id)
    {
        public string Name { get; private set; } = Name;
        public string Password { get; private set; } = password;
        public string Email { get; private set; } = email;
        public Role Role { get; private set; } = role;
    }
}
