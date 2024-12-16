using DistributeSystem.Contract.Abstractions.Message;
using static DistributeSystem.Contract.Services.V1.Product.Response;

namespace DistributeSystem.Contract.Services.V1.Product;

public static class Query
{
    public record GetProductsQuery() : IQuery<List<ProductResponse>>;
    public record GetProductByIdQuery(Guid Id) : IQuery<ProductResponse>;
}
