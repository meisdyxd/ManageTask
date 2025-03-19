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
            services
                .AddDbContext(configuration)
                .AddRedisCache(configuration)
                .AddRepositories();
            return services;
        }
        private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPostgresDataAccess<DataContext, DataContextConfigurator>();

            return services;
        }
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>()
                .AddScoped<ITaskRepository, TaskRepository>();
            return services;
        }
        private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis options are not configured");

            services.AddStackExchangeRedisCache(options =>
                options.Configuration = redisConnectionString);

            return services;
        }
    }
}
