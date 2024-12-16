using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeSystem.Contract.Abstractions.Shared
{
    public sealed class ValidationResult<T> : Result<T>, IValidationResult
    {
        private ValidationResult(Error[] errors) : 
            base(default, false, IValidationResult.ValidationError)
            => Errors = errors;

        public Error[] Errors { get; }
        public static ValidationResult<T> WithErrors(Error[] errors) => new(errors);

    }
}
