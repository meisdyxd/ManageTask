﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ManageTask.Infrastructure.Data.Configurations;

namespace ManageTask.Infrastructure.Data.Configurations.RegistrationExtension
{
    public static class DbContextRegistrationExtension
    {
        public static IServiceCollection AddPostgresDataAccess<TContext, TContextConfiguration>(this IServiceCollection services)
            where TContext: DbContext
            where TContextConfiguration: class, IDbContextOptionsConfigurator<TContext>
        {
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<TContext>(Configure<TContext>);
            services.AddSingleton<IDbContextOptionsConfigurator<TContext>, TContextConfiguration>()
                .AddScoped<DbContext>(t => t.GetRequiredService<TContext>());
            return services;
        }
        internal static void Configure<TContext>(IServiceProvider sp, DbContextOptionsBuilder optionsBuilder) where TContext : DbContext
        {
            var configuration = sp.GetRequiredService<IDbContextOptionsConfigurator<TContext>>();
            configuration.Configure((DbContextOptionsBuilder < TContext >) optionsBuilder);
        }
    }
}
