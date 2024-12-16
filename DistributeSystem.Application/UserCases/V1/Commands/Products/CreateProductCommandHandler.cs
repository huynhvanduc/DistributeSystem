using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Product;
using DistributeSystem.Domain.Abstractions.Repositories;
using DistributeSystem.Domain.Entities;

namespace DistributeSystem.Application.UserCases.V1.Commands.Products;

public sealed class CreateProductCommandHandler : ICommandHandler<Command.CreateProductCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Product, Guid> _repository;

    public CreateProductCommandHandler(IRepositoryBase<Domain.Entities.Product, Guid> repository)
    {
        _repository = repository;   
    }

    public async Task<Result> Handle(Command.CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.CreateProduct(Guid.NewGuid(), request.Name, request.Price, request.Description);
        _repository.Add(product);
        return Result.Success();
    }
}
