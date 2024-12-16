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
    public class PerfomancePipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;

        public PerfomancePipelineBehavior(ILogger<TRequest> logger)
        {
            _timer = new Stopwatch();
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();
            var response = await next();
            _timer.Stop();

            var elapsedMiliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMiliseconds <= 5000)
                return response;

            var requestName = typeof(TRequest).Name;
            _logger.LogWarning($"Long time running - Request Details: {requestName} ({elapsedMiliseconds} milisecon) {request}");

            return response;
        }
    }
}
