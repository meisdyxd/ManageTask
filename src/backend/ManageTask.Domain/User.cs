using ManageTask.Domain.Abstractions;

namespace ManageTask.Domain
{
    public class User:
        EntityObject<Guid>
    {
        public User(Guid id, string name, string email, string password, Role role) : base(id)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }
        public User() : base(Guid.NewGuid()) // Или другой способ инициализации Id
        {
        }
        public string Name { get; set; }
        public string Password { get;  set; }
        public string Email { get;  set; }
        public Role Role { get;  set; }
    }
}
