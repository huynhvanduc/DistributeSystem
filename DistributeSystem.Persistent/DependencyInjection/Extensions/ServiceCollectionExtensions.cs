﻿using DistributeSystem.Domain.Abstractions.Repositories;
using DistributeSystem.Domain.Abstractions;
using DistributeSystem.Domain.Entities.Identity;
using DistributeSystem.Persistence.DependencyInjection.Options;
using DistributeSystem.Persistence.Interceptors;
using DistributeSystem.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DistributedSystem.Persistence;

namespace DistributeSystem.Persistence.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSQLConfiguration(this IServiceCollection services)
        {
            services.AddDbContextPool<DbContext, ApplicationDbContext>((provider, builder) =>
            {
                var outBoxInterceptor = provider.GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();
                var auditableInterceptor = provider.GetService<UpdateAuditableEntitiesInterceptor>();

                var configuration = provider.GetRequiredService<IConfiguration>();
                var options = provider.GetRequiredService<IOptionsMonitor<SqlServerRetryOptions>>();

                #region ====== SQL-SERVER-STRATEGY-1 ====
                builder
                .EnableDetailedErrors(true)
                .EnableSensitiveDataLogging(true)
                .UseLazyLoadingProxies(true)
                .UseSqlServer(
                    connectionString: configuration.GetConnectionString("ConnectionStrings"),
                    sqlServerOptionsAction: optionBuider
                    => optionBuider.ExecutionStrategy(
                        dependencies => new SqlServerRetryingExecutionStrategy(
                            dependencies: dependencies,
                            maxRetryCount: options.CurrentValue.MaxRetryCount,
                            maxRetryDelay: options.CurrentValue.MaxRetryDelay,
                            errorNumbersToAdd: options.CurrentValue.ErrorNumbersToAdd))
                    .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name))
                    .AddInterceptors(outBoxInterceptor,
                        auditableInterceptor);

                #endregion
            });

            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Lockout.AllowedForNewUsers = true; // Default true
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // Default 5
                opt.Lockout.MaxFailedAccessAttempts = 3; // Default 5
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = true; // Default true
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // Default 5
                options.Lockout.MaxFailedAccessAttempts = 3; // Default 5
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.AllowedForNewUsers = true;
            });
        }

        public static void AddInterceptorDbContext(this IServiceCollection services)
        {
            services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
            services.AddSingleton<UpdateAuditableEntitiesInterceptor>();
        }

        public static void AddRepositoryBaseConfiguration(this IServiceCollection services)
        {
            services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
            services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));

            services.AddTransient(typeof(IUnitOfWorkDbContext<>), typeof(EFUnitOfWorkDbContext<>));
            services.AddTransient(typeof(IRepositoryBaseDbContext<,,>), typeof(RepositoryBaseDbContext<,,>));
        }

        public static OptionsBuilder<SqlServerRetryOptions> ConfigureSqlServerRetryOptions(this IServiceCollection services, IConfigurationSection section)
            => services
                .AddOptions<SqlServerRetryOptions>()
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();
    }
}
