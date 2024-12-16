using Serilog;
using DistributeSystem.Application.DependencyInjection.Extensions;
using DistributeSystem.API.Middlewares;
using Carter;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using DistributeSystem.API.DependencyInjections.Extensions;
using DistributeSystem.Persistence.DependencyInjection.Extensions;
using DistributeSystem.Persistence.DependencyInjection.Options;
using DistributeSystem.Infrastructure.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Serilog 
Log.Logger = new LoggerConfiguration()
    .ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();
#endregion

#region swagger
builder.Services
    .AddSwaggerGenNewtonsoftSupport()
    .AddFluentValidationRulesToSwagger()
    .AddEndpointsApiExplorer()
    .AddSwaggerAPI();

builder.Services
    .AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVVV";
        options.SubstituteApiVersionInUrl = true;
    });
#endregion

builder.Services.AddJwtAuthenicationJWT(builder.Configuration);

#region Infrastructor
builder.Services.AddMasstransitRabbitMQ(builder.Configuration);
builder.Services.AddQuartzInfratructure();
builder.Services.AddInfrastructureServices();
builder.Services.AddRedisService(builder.Configuration);
#endregion

#region Application
builder.Services.AddMediaRApplication();
builder.Services.AddAutoMapperApplication();
#endregion


#region middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
#endregion

// Configure Options and SQL => Remember mapcarter
builder.Services.AddInterceptorDbContext();
builder.Services.ConfigureSqlServerRetryOptions(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddMediatRInfrastructure();
builder.Services.AddSQLConfiguration();
builder.Services.AddRepositoryBaseConfiguration();
builder.Services.ConfigureServicesInfrastructure(builder.Configuration);

#region Carter
builder.Services.AddCarter();
#endregion



var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
app.ConfigSwagger(); // => After MapCarter => Show Version

try
{
await app.RunAsync();
Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
await app.StopAsync();
}
finally
{
Log.CloseAndFlush();
await app.DisposeAsync();
}

public partial class Program { }
