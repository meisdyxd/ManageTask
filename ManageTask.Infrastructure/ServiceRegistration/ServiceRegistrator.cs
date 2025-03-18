using ManageTask.Application.Abstractions.Data;
using ManageTask.Infrastructure.Data.Configurations.RegistrationExtension;
using ManageTask.Infrastructure.Data.Contexts;
using ManageTask.Infrastructure.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static ManageTask.Infrastructure.Data.Contexts.DataContextConfiguration;

namespace ManageTask.Infrastructure.ServiceRegistration
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositories()
                .AddDbContext(configuration);
            return services;
        }
        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPostgresDataAccess<DataContext, DataContextConfigurator>();

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
