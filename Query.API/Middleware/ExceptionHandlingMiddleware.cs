using DistributeSystem.Contract.Abstractions.Shared;
using Query.Domain.Exceptions;
using System.Text.Json;

namespace Query.API.Middlewares;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _loger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> loger)
        => _loger = loger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _loger.LogError(e, e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);

        var response = new
        {
            title = GetTitle(exception),
            status = statusCode,
            detail = exception.Message,
            errors = GetErrors(exception)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private IReadOnlyCollection<Error> GetErrors(Exception exception)
    {
        IReadOnlyCollection<Error> errors = null;
        if (exception is Query.Application.Exceptions.ValidationException validationException)
            errors = validationException.Errors;

        return errors;
    }
    

    private string GetTitle(Exception exception)
    => exception switch
    {
        DomainException domainException => domainException.Title,
        _ => "server error"
    };

    private int GetStatusCode(Exception exception)
     => exception switch
     {
         ProductException.ProductFieldException => StatusCodes.Status406NotAcceptable,
         BadRequestException => StatusCodes.Status400BadRequest,
         NotFoundException => StatusCodes.Status404NotFound,
         FormatException => StatusCodes.Status422UnprocessableEntity,
         _ => StatusCodes.Status500InternalServerError
     };
}
