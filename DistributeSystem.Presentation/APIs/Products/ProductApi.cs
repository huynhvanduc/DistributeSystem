using Carter;
using DistributeSystem.Contract.Services.V1.Product;
using DistributeSystem.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;


namespace DistributeSystem.Presentation.APIs.Products
{
    public class ProductApi : ApiEndpoint, ICarterModule
    {
        private const string BaseUrl = "/api/v{version:apiVersion}/products";
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group1 = app.NewVersionedApi("products")
                .MapGroup(BaseUrl).HasApiVersion(1);

            group1.MapPost(string.Empty,  CreateProductsV1);
            //group1.MapGet(string.Empty,  GetProductsV1);
            //group1.MapGet("{productId}",  GetProductByIdV1);
            group1.MapDelete("{productId}", DeleteProductV1);
            group1.MapPut("{productId}",  UpdateProductV1);
        }

        public static async Task<IResult> CreateProductsV1(ISender sender, [FromBody] Command.CreateProductCommand CreateProduct)
        {
            var result = await sender.Send(CreateProduct);

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }

        public static async Task<IResult> GetProductsV1(ISender sender)
        {
            var result = await sender.Send(new Query.GetProductsQuery());

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }

        public static async Task<IResult> GetProductByIdV1(ISender sender, Guid Id)
        {
            var result = await sender.Send(new Query.GetProductByIdQuery(Id));

            if (result.IsFailure)
                return HandlerFailure(result);

            return Results.Ok(result);
        }

        public static async Task<IResult> UpdateProductV1(ISender sender, Guid productId,[FromBody] Command.UpdateProductCommand updateProduct)
        {
            var product = new Command.UpdateProductCommand(productId, updateProduct.Name, updateProduct.Price, updateProduct.Description);
            var result = await sender.Send(product);    
            return Results.Ok(result);
        }

        public static async Task<IResult> DeleteProductV1(ISender sender, Guid productId)
        {
            var product = await sender.Send(new Command.DeleteProductCommand(productId));
            return Results.Ok(product);
        }
    }
}
