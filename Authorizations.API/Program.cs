using Carter;
using Authorizations.API.DependencyInjection.Extensions;
using Authorizations.API.Middleware;
using Authorizations.Application.DependencyInjection.Extensions;
using Authorizations.Infrastructor.DependencyInjection.Extensions;
using Serilog;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add configuration

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

// Add Carter module
builder.Services.AddCarter();

// Add Swagger
builder.Services
        .AddSwaggerGenNewtonsoftSupport()
        .AddFluentValidationRulesToSwagger()
        .AddEndpointsApiExplorer()
        .AddSwaggerAPI();

builder.Services
    .AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddJwtAuthenticationAPI(builder.Configuration);
builder.Services.AddMediatRApplication();
builder.Services.AddServicesInfrastructure();
builder.Services.AddRedisInfrastructure(builder.Configuration);

// Add Middleware => Remember using middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

// Using middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseHttpsRedirection(); // => Use in production environment

app.UseAuthentication(); // This need to be added before UseAuthorization
app.UseAuthorization();

// Add API Endpoint with carter module
app.MapCarter();

// Configure the HTTP request pipeline. 
if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
app.UseSwaggerAPI(); // => After MapCarter => Show Version

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