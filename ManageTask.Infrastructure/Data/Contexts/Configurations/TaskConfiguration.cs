using ManageTask.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageTask.Infrastructure.Data.Contexts.Configurations
{
    public class TaskConfiguration: IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => t.CreatedById);
            builder.HasIndex(t => t.AssignedToId);
            builder.Property(t => t.Status)
                .HasConversion<string>();
        }
    }
}
