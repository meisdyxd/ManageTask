namespace ManageTask.Contracts.ApiContracts
{
    public class UserPublic
    {
        public UserPublic(Guid id, string name, string email, string role)
        {
            Id = id;
            Name = name;
            Email = email;
            Role = role;
        }
        public UserPublic() { }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
