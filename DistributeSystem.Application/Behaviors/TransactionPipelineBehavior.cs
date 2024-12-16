using DistributeSystem.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DistributeSystem.Application.Behaviors
{
    public sealed class TransactionPipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ApplicationDbContext _dbContext;

        public TransactionPipelineBehavior(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!IsCommand())
                return await next();

            var strategy = _dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                {
                    var response = await next();
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return response;
                }
            });
        }

        private bool IsCommand()
       => typeof(TRequest).Name.EndsWith("Command");
    }
}
