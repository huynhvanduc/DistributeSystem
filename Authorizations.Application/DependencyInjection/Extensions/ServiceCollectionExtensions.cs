using Microsoft.Extensions.DependencyInjection;
using Authorizations.Application.Abstractions;

namespace Authorizations.Application.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRApplication(this IServiceCollection services)
        => services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly));
}
