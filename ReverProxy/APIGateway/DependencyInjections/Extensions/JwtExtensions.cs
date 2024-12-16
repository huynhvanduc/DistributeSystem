using APIGateway.DependencyInjections.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace APIGateway.DependencyInjections.Extensions
{
    public static class JwtExtensions
    {
        public static void AddJwtAuthenicationJWT(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                JwtOption jwtOption = new JwtOption();
                configuration.GetSection(nameof(jwtOption)).Bind(jwtOption);

                o.SaveToken = true;

                var key = Encoding.UTF8.GetBytes(jwtOption.SecretKey);

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOption.Issuer,
                    ValidAudience = jwtOption.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                        }
                        return Task.CompletedTask;
                    }
                };

                //o.EventsType = typeof(CustomJwtBearerEvents);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("authPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });
            // services.AddScoped<CustomJwtBearerEvents>();
        }
    }
}
