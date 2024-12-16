using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Product;
using MongoDB.Driver;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;

namespace Query.Application.UseCases.V1.Queries;

public class GetProductQueryHandler : IQueryHandler<DistributeSystem.Contract.Services.V1.Product.Query.GetProductsQuery, List<Response.ProductResponse>>
{
    private readonly IMongoRepository<ProductProjection> _productRepository;
    public GetProductQueryHandler(IMongoRepository<ProductProjection> productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<Result<List<Response.ProductResponse>>> Handle(DistributeSystem.Contract.Services.V1.Product.Query.GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products =  await _productRepository.AsQueryable().ToListAsync();
        var result = new List<Response.ProductResponse>();

        foreach(var item in products)
        {
            result.Add(new Response.ProductResponse(item.DocumentId, item.Name, item.Price, item.Description));
        }

        return Result.Success(result);
    }
}
