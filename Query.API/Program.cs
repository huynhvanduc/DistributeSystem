using Carter;
using Query.API.DependencyInjections.Extensions;
using Query.API.Middlewares;
using Serilog;
using Query.Persistence.DependencyInjecion.Extentions;
using Query.Infrastructure.DependencyInjection.Extensions;
using Query.Application.DependencyInjection.Extensions;

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

builder.Services.AddCarter();

builder.Services
    .AddSwaggerGenNewtonsoftSupport()
    .AddEndpointsApiExplorer()
    .AddSwaggerAPI();

builder.Services
    .AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });



builder.Services.AddMediatRApplication();

builder.Services.ConfigureServicesInfrestructure(builder.Configuration);
builder.Services.AddMasstransitRabbitMQ(builder.Configuration);

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddServicesPersistence();


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapCarter();

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
    app.UseSwaggerAPI();

app.Run();
