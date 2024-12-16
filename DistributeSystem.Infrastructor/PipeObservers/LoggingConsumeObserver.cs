﻿using MassTransit;
using Serilog;

namespace DistributeSystem.Infrastructure.PipeObservers;

public class LoggingConsumeObserver : IConsumeObserver
{
    public Task PostConsume<T>(ConsumeContext<T> context)
       where T : class
       => Task.CompletedTask;

    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        where T : class
        => Task.CompletedTask;

    public async Task PreConsume<T>(ConsumeContext<T> context) 
        where T : class
    {
        await Task.Yield();

        Log.Information("Consuming {Message} message from {Namespace}, CorrelationId: {CorrelationId}",
            typeof(T).Name, typeof(T).Namespace, context.CorrelationId);
    }
    
}