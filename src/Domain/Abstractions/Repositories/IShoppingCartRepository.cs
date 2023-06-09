using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IShoppingCartRepository
{
    Task AddAsync(ShoppingCart shoppingCart);

    Task<ShoppingCart?> GetShoppingCartByUserIdAsync(Guid userId);
}