using DistributeSystem.Contract.Services.V1.Product;
using DistributeSystem.Domain.Abstractions;
using DistributeSystem.Domain.Abstractions.Aggregates;
using DistributeSystem.Domain.Abstractions.Entities;
using DistributeSystem.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Persistence
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public EFUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //ConvertDomainEventsToOutboxMessages();
            //UpdateAuditableEntities();
            await _context.SaveChangesAsync();
        }

        private void ConvertDomainEventsToOutboxMessages()
        {
            var outboxMessage = _context.ChangeTracker
                .Entries<Domain.Abstractions.Aggregates.AggregateRoot<Guid>>()
                .Select(x => x.Entity)
                .SelectMany(aggregateRoot =>
                {
                    var domainEvents = aggregateRoot.GetDomainEvents();

                    aggregateRoot.ClearDomainEvents();

                    return domainEvents;
                })
                .Select(domainEvent => new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    OccurredOnUtc = DateTime.UtcNow,
                    Type = domainEvent.GetType().Name,
                    Content = JsonConvert.SerializeObject(
                      domainEvent,
                      new JsonSerializerSettings
                      {
                          TypeNameHandling = TypeNameHandling.All
                      }
                    )
                }).ToList();
        }

        private void UpdateAuditableEntities()
        {
            IEnumerable<EntityEntry<IAuditableEntity>> entries
                = _context.ChangeTracker
                .Entries<IAuditableEntity>();

            foreach(EntityEntry<IAuditableEntity> entityEntry in entries)
            {
                if(entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(a => a.CreatedOnUtc).CurrentValue = DateTime.Now;
                }

                if(entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(a => a.ModifiedOnUtc).CurrentValue = DateTime.Now;
                }
            }
        }
    }
}
