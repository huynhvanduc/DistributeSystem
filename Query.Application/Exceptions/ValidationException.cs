using DistributeSystem.Contract.Abstractions.Shared;
using Query.Domain.Exceptions;

namespace Query.Application.Exceptions;

public sealed class ValidationException : DomainException
{
    public  IReadOnlyCollection<Error> Errors { get; set; }
    public ValidationException(IReadOnlyCollection<Error> errors) : base("Validation", "One or more validation errors occurred")
    {
        Errors = errors;
    }
}

