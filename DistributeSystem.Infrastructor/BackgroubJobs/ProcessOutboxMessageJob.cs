using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Persistence;
using DistributeSystem.Persistence.Outbox;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Newtonsoft.Json;
using DistributeSystem.Contract.Services.V1.Product;

namespace DistributeSystem.Infrastructure.BackgroubJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessageJob : IJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public ProcessOutboxMessageJob(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        List<OutboxMessage> messages = await _dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        foreach (OutboxMessage outboxMessage in messages)
        {
            IDomainEvent? domainEvent = JsonConvert
                .DeserializeObject<IDomainEvent>(outboxMessage.Content,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

            if (domainEvent is null)
                continue;

            try
            {
                switch (domainEvent.GetType().Name)
                {
                    case nameof(DomainEvent.ProductCreated):
                        var productCreated = JsonConvert.DeserializeObject<DomainEvent.ProductCreated>(
                                outboxMessage.Content,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All
                                });
                        await _publishEndpoint.Publish(productCreated, context.CancellationToken);
                        break;

                    case nameof(DomainEvent.ProductUpdated):
                        var productUpdate = JsonConvert.DeserializeObject<DomainEvent.ProductUpdated>(
                                outboxMessage.Content,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All
                                });
                        await _publishEndpoint.Publish(productUpdate, context.CancellationToken);
                        break;

                    case nameof(DomainEvent.ProductDeleted):
                        var productDeleted = JsonConvert.DeserializeObject<DomainEvent.ProductDeleted>(
                                    outboxMessage.Content,
                                    new JsonSerializerSettings
                                    {
                                        TypeNameHandling = TypeNameHandling.All
                                    });
                        await _publishEndpoint.Publish(productDeleted, context.CancellationToken);
                        break;

                    default:
                        break;
                }

                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception)
            {

                throw;
            }
        }
        await _dbContext.SaveChangesAsync();
    }
}
