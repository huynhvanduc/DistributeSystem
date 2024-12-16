namespace DistributeSystem.Domain.Exceptions;

public static class ProductException
{
    public class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(Guid productId) : base($"The product with {productId} was not found")
        {
        }
    }

    public class ProductFieldException : NotFoundException
    {
        public ProductFieldException(string productField)
            : base($"The product with the field {productField} is not correct.") { }
    }
}
