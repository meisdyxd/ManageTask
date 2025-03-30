using Microsoft.EntityFrameworkCore;

namespace ManageTask.Infrastructure.Data.Configurations
{
    public interface IDbContextOptionsConfigurator<TContext> where TContext: DbContext
    {
        public void Configure(DbContextOptionsBuilder<TContext> options);
    }
}
