using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Query.Presentation.Abstractions;

namespace Query.Presentation.APIs;

public class ProductApi : ApiEndpoint, ICarterModule
{
    private const string BaseUrl = "/api/v{version:apiVersion}/products";
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group1 = app.NewVersionedApi("products")
            .MapGroup(BaseUrl).HasApiVersion(1);

        group1.MapGet(string.Empty, GetProductsV1);
        group1.MapGet("{productId}", GetProductByIdV1);
    }

    public static async Task<IResult> GetProductsV1(ISender sender)
    {
        var result = await sender.Send(new DistributeSystem.Contract.Services.V1.Product.Query.GetProductsQuery());

        if (result.IsFailure)
            return HandlerFailure(result);

        return Results.Ok(result);
    }

    public static async Task<IResult> GetProductByIdV1(ISender sender, Guid productId)
    {
        var result = await sender.Send(new DistributeSystem.Contract.Services.V1.Product.Query.GetProductByIdQuery(productId));

        if (result.IsFailure)
            return HandlerFailure(result);

        return Results.Ok(result);
    }
}
