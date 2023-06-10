using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IShoppingCartItemRepository
{
    Task AddAsync(ShoppingCartItem shoppingCartItem,CancellationToken cancellationToken = default);
    void Update(ShoppingCartItem shoppingCartItem);
}