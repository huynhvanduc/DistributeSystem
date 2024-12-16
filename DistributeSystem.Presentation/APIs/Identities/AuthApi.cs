using Carter;
using DistributeSystem.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DistributeSystem.Presentation.APIs.Identities;

public class AuthApi : ApiEndpoint, ICarterModule
{
    private const string baseUrl = "/api/v{version:apiVersion}/auth";
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group1 = app.NewVersionedApi("Authenication")
            .MapGroup(baseUrl).HasApiVersion(1).RequireAuthorization();

        group1.MapPost("login", AuthenicationV1).AllowAnonymous();
    }

    private async Task<IResult> AuthenicationV1(ISender sender, [FromBody] Contract.Services.V1.Identity.Query.Login login)
    {
        var result = await sender.Send(login);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Results.Ok(result);
    }
}
