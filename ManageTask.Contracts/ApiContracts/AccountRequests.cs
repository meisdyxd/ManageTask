using ManageTask.Domain;

namespace ManageTask.Contracts.ApiContracts
{
    public enum PublicRole
    {
        User = Role.User,
        Manager = Role.Manager,
        Admin = Role.Admin
    }
    public record RegisterRequest(string Username, string Email, string Password, PublicRole Role);
    public record LoginRequest(string Email, string Password);
}
