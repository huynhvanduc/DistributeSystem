using MassTransit;

namespace DistributeSystem.Contract.Abstractions.Message;

[ExcludeFromTopology]
public interface IDomainEvent /*: INotification*/
{
    public Guid IdEvent { get; init; }
}
