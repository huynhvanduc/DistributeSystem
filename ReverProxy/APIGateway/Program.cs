using APIGateway.DependencyInjections.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxyApiGateway(builder.Configuration);

builder.Services.AddJwtAuthenicationJWT(builder.Configuration);

var app = builder.Build();

app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();



app.Run();
