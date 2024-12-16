using DistributeSystem.Contract.Abstractions.Shared;
using MediatR;

namespace DistributeSystem.Contract.Abstractions.Message;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
