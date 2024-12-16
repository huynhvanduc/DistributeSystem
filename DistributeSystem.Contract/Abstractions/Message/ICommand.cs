using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using MediatR;

namespace DistributeSystem.Contract.Abstractions.Message
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>> { 
    }
}
