using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Application.Behaviors
{
    public class TracingPinelineBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;

        public TracingPinelineBehavior(ILogger<TRequest> logger)
        {
            _timer = new Stopwatch();
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();
            var response = await next();
            _timer.Stop();

            var elappsedMiliseconds = _timer.ElapsedMilliseconds;
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning($"Time running - Request Details: {requestName} ({elappsedMiliseconds} milisecon) {request}");
           
            return response;
        }
    }
}
