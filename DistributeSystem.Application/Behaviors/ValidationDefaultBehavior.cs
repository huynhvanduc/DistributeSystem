using FluentValidation;
using MediatR;

namespace DistributeSystem.Application.Behaviors;

public sealed class ValidationDefaultBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationDefaultBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var errorDictionary = _validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(x => new { x.PropertyName, x.ErrorMessage })
            .Select(x => x.FirstOrDefault())
            .ToList();

        if (errorDictionary.Any())
            throw new ValidationException(errorDictionary);

        return await next();
    }
}
