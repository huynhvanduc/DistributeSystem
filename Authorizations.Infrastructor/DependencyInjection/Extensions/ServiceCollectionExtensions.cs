using Authorizations.Application.Abstractions;
using Authorizations.Infrastructor.Authenication;
using Authorizations.Infrastructor.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authorizations.Infrastructor.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServicesInfrastructure(this IServiceCollection services)
    => services.AddTransient<IJwtTokenService, JwtTokenService>()
        .AddTransient<ICacheService, CacheService>();

    public static void AddRedisInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            redisOptions.Configuration = connectionString;
        });
    }
}
