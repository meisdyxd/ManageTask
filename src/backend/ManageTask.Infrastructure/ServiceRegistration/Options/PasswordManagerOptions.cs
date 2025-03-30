namespace ManageTask.Infrastructure.ServiceRegistration.Options
{
    public class PasswordManagerOptions
    {
        public required string Salt { get; set; }
        public bool EnhancedEntropy { get; set; } = true;
    }
}
