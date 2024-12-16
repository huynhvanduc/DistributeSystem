using DistributeSystem.Contract.Abstractions.Shared;
using MediatR;

namespace DistributeSystem.Contract.Abstractions.Message;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
