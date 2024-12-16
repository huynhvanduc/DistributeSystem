namespace Query.Domain.Exceptions;

public abstract class BadRequestException : DomainException
{
    protected BadRequestException(string message) : base("Bad request", message)
    {
    }
}
