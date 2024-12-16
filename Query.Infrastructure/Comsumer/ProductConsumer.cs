using DistributeSystem.Contract.Services.V1.Product;
using MediatR;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;
using Query.Infrastructure.Abstractions;

namespace Query.Infrastructure.Comsumer;

public static class ProductConsumer
{
    public class ProductCreateConsumer : Consumer<DomainEvent.ProductCreated>
    {
        public ProductCreateConsumer(ISender sender, IMongoRepository<EventProjection> eventRepository) : base(sender, eventRepository)
        {
        }
    }

    public class ProductDeleteConsumer : Consumer<DomainEvent.ProductDeleted>
    {
        public ProductDeleteConsumer(ISender sender, IMongoRepository<EventProjection> eventRepository) : base(sender, eventRepository)
        {
        }
    }

    public class ProductUpdateConsumer : Consumer<DomainEvent.ProductUpdated>
    {
        public ProductUpdateConsumer(ISender sender, IMongoRepository<EventProjection> eventRepository) : base(sender, eventRepository)
        {
        }
    }
}