using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Infrastructure.DependencyInjection.Options
{
    public  class MessageBusOptions
    {
        [Required, Range(1, 10)] public int RetryLimit { get; init; }
        [Required, Timestamp] public TimeSpan InitialInterval { get; init; }
        [Required, Timestamp] public TimeSpan IntervalIncrement { get; init; }
    }
}
