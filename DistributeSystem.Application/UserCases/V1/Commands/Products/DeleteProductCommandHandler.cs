using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Product;
using DistributeSystem.Domain.Abstractions.Repositories;
using DistributeSystem.Domain.Entities;
using DistributeSystem.Domain.Exceptions;

namespace DistributeSystem.Application.UserCases.V1.Commands.Products;

public class DeleteProductCommandHandler : ICommandHandler<Command.DeleteProductCommand>
{
    private readonly IRepositoryBase<Product, Guid> _repository;

    public DeleteProductCommandHandler(IRepositoryBase<Product, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(Command.DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.FindByIdAsync(request.Id) ?? throw new ProductException.ProductNotFoundException(request.Id);
        product.Delete();
        _repository.Remove(product);
        return Result.Success();
    }
}
