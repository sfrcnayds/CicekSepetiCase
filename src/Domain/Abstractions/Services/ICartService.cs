namespace Domain.Abstractions.Services;

public interface ICartService
{
    void AddToCart(Guid productId, int quantity);
}