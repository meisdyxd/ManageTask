using ManageTask.Application.Abstractions.Data;
using ManageTask.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ManageTask.Infrastructure.ServiceRegistration
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddRepositories();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>()
                .AddScoped<ITaskRepository, TaskRepository>();
            return services;
        }
    }
}
