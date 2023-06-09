using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IShoppingCartItemRepository
{
    Task<ShoppingCartItem?> GetByIdAsync(Guid shoppingCartItemId);
    Task AddAsync(ShoppingCartItem shoppingCartItem);
    
    Task AddRangeAsync(IEnumerable<ShoppingCartItem> shoppingCartItems);
}