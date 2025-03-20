using ManageTask.Application.Abstractions.Services;
using ManageTask.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ManageTask.Application.ServiceRegistration
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddServices();
            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskService, TaskService>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
