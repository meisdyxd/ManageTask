namespace ManageTask.Contracts.ApiContracts
{
    public record UserPublic(Guid Id, string Username, string Email, string Role);
}
