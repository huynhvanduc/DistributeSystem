using DistributeSystem.Contract.Abstractions.Message;
using MassTransit;
using MediatR;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;

namespace Query.Infrastructure.Abstractions;

public abstract class Consumer<TMassage> : IConsumer<TMassage>
    where TMassage : class, IDomainEvent
{
    private readonly ISender Sender;
    private readonly IMongoRepository<EventProjection> _eventRepository;

    protected Consumer(ISender sender, IMongoRepository<EventProjection> eventRepository)
    {
        Sender = sender;
        _eventRepository = eventRepository;
    }

    public async Task Consume(ConsumeContext<TMassage> context)
    {
        var eventProjection = await _eventRepository.FindOneAsync(x => x.EventId == context.Message.IdEvent);

        if (eventProjection is null)
        {
            await Sender.Send(context.Message);
            eventProjection = new EventProjection()
            {
                EventId = context.Message.IdEvent,
                Name = context.Message.GetType().Name,
                Type = context.Message.GetType().Name
            };

            await _eventRepository.InsertOneAsync(eventProjection);
        }
    }
}

