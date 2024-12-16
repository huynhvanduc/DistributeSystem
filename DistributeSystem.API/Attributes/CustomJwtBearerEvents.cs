using DistributedSystem.Application.Abstractions;
using DistributeSystem.Contract.Services.V1.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DistributeSystem.API.Attributes;

public class CustomJwtBearerEvents : JwtBearerEvents
{
    private readonly ICacheService _cacheService;

    public CustomJwtBearerEvents(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        if(context.SecurityToken is JwtSecurityToken accessToken)
        {
            var requestToken = accessToken.RawData.ToString();

            var emailKey = accessToken.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;

            var authenicated = await _cacheService.GetAsync<Response.Authenicated>(emailKey);

            if(authenicated is null || authenicated.AccessToken != requestToken)
            {
                context.Response.Headers.Add("IS-TOKEN-REVOKED", "True");
                context.Fail("Authenication fail. Token has been revoked!");
            }
        }
        else
        {
            context.Fail("Autheniation fail");
        }
    }
}
