using DistributeSystem.Application.Behaviors;
using DistributeSystem.Application.Mapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DistributeSystem.Application.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediaRApplication(this IServiceCollection services)
    {
        return services.AddMediatR(cfg 
        => cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly))
           //.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationDefaultBehavior<,>))
          .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
          .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerfomancePipelineBehavior<,>))
          .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionPipelineBehavior<,>))
          //.AddTransient(typeof(IPipelineBehavior<,>), typeof(TracingPipelineBehavior<,>))
          .AddValidatorsFromAssembly(Contract.AssemblyReference.Assembly, includeInternalTypes: true);
    }

    public static IServiceCollection AddAutoMapperApplication(this IServiceCollection services)
    {
        return services.AddAutoMapper(typeof(ServiceProfile));
    }
}
