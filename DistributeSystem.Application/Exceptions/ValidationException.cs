using DistributeSystem.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Application.Exceptions
{
    public sealed class ValidationException : DomainException
    {
        public IReadOnlyCollection<ValidationError> Errors { get; }

        public ValidationException(IReadOnlyCollection<ValidationError> errors) 
            : base("Validation Failure", "One or more validation errors occurred")
        => Errors = errors;
    }

    public record ValidationError(string PropertyName, string ErrorMessage);
}
