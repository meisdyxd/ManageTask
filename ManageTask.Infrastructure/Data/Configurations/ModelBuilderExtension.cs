using ManageTask.Infrastructure.Data.Contexts.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ManageTask.Infrastructure.Data.Configurations
{
    public static class ModelBuilderExtension
    {
        public static ModelBuilder ApplyAllConfigurations(this ModelBuilder modelBuilder)
        {
            return modelBuilder
                .ApplyUserConfiguration()
                .ApplyTaskConfiguration();
        }
        public static ModelBuilder ApplyUserConfiguration(this ModelBuilder modelBuilder)
        {
            return modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
        public static ModelBuilder ApplyTaskConfiguration(this ModelBuilder modelBuilder)
        {
            return modelBuilder.ApplyConfiguration(new TaskConfiguration());
        }
    }
}
