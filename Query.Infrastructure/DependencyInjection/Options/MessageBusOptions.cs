using System.ComponentModel.DataAnnotations;

namespace Query.Infrastructure.DependencyInjecion.Options
{
    public  class MessageBusOptions
    {
        [Required, Range(1, 10)] public int RetryLimit { get; init; }
        [Required, Timestamp] public TimeSpan InitialInterval { get; init; }
        [Required, Timestamp] public TimeSpan IntervalIncrement { get; init; }
    }
}
