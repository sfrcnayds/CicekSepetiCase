namespace Application.Exceptions;

public class InsufficientStockAvailableException : Exception
{
    public InsufficientStockAvailableException()
        : base("Insufficient stock available.")
    {
    }

    public InsufficientStockAvailableException(string message)
        : base(message)
    {
    }
}