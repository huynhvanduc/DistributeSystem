using Carter;
using DistributeSystem.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Presentation.APIs.Identities
{
    public class TokenApi : ApiEndpoint, ICarterModule
    {
        private const string baseUrl = "/api/v{version:apiVersion}/auth";
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group1 = app.NewVersionedApi("Authenication")
                       .MapGroup(baseUrl).HasApiVersion(1).RequireAuthorization();

            group1.MapPost("refresh", RefreshV1);
            group1.MapPost("revoke", RevokeV1);
        }

        public static async Task<IResult> RevokeV1(ISender sender, HttpContext context)
        {
            var AccessToken = await context.GetTokenAsync("access_token");

            var result = sender.Send(new Contract.Services.V1.Identity.Command.Revoke(AccessToken));
            return Results.Ok(result);
        }

        private static async Task<IResult> RefreshV1(ISender sender, HttpContext context, [FromBody] Contract.Services.V1.Identity.Query.Token token)
        {
            var AccessToken = await context.GetTokenAsync("access_token");
            var result = await sender.Send(new Contract.Services.V1.Identity.Query.Token(AccessToken, token.RefreshToken));

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }
    }
}
