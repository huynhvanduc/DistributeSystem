using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Product;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;

namespace Query.Application.UseCases.V1.Queries
{
    public class GetProductQueryByIdHandler : IQueryHandler<DistributeSystem.Contract.Services.V1.Product.Query.GetProductByIdQuery, Response.ProductResponse>
    {
        private readonly IMongoRepository<ProductProjection> _productRepository;
        public GetProductQueryByIdHandler(IMongoRepository<ProductProjection> productRepository)
        { 
            _productRepository = productRepository;
        }

        public async Task<Result<Response.ProductResponse>> Handle(DistributeSystem.Contract.Services.V1.Product.Query.GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindOneAsync(p => p.DocumentId == request.Id) 
                ?? throw new ArgumentNullException(nameof(request.Id));

            var result = new Response.ProductResponse(product.DocumentId, product.Name, product.Price, product.Description);

            return Result.Success(result);
        }
    }
}
