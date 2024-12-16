using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Product;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;

namespace Query.Application.UseCases.V1.Commands;
public class ProjectProductDetailsWhenProductChangedEventHandler
        : ICommandHandler<DomainEvent.ProductCreated>,
        ICommandHandler<DomainEvent.ProductUpdated>,
        ICommandHandler<DomainEvent.ProductDeleted>
{
    private readonly IMongoRepository<ProductProjection> _productRepository;

    public ProjectProductDetailsWhenProductChangedEventHandler(IMongoRepository<ProductProjection> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result> Handle(DomainEvent.ProductCreated request, CancellationToken cancellationToken)
    {
        var product = new ProductProjection()
        {
            DocumentId = request.Id,
            Name = request.Name,
            Price = request.Price,
            Description = request.Description
        };

        await _productRepository.InsertOneAsync(product);

        return Result.Success();
    }

    public async Task<Result> Handle(DomainEvent.ProductUpdated request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindOneAsync(p => p.DocumentId == request.Id)
            ?? throw new ArgumentException();

        product.Name = request.Name;
        product.Price = request.Price;
        product.Description = request.Description;
        product.ModifiedOnUtc = DateTime.UtcNow;

        await _productRepository.ReplaceOneAsync(product);

        return Result.Success();
    }

    public async Task<Result> Handle(DomainEvent.ProductDeleted request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindOneAsync(p => p.DocumentId == request.Id)
            ?? throw new ArgumentException();

        await _productRepository.DeleteOneAsync(p => p.DocumentId == request.Id);

        return Result.Success();
    }
}
