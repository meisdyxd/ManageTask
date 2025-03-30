using ManageTask.Infrastructure.Data.Configurations;
using ManageTask.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManageTask.Infrastructure.Data.Contexts
{
    public class DataContext(DbContextOptions<DataContext> options): DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CustomModelBuilder.OnModelCreating(modelBuilder);
            modelBuilder.ApplyAllConfigurations();
        }
    }
}
