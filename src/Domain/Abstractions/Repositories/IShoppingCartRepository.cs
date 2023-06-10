using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IShoppingCartRepository
{
    Task AddAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
    Task<ShoppingCart?> GetShoppingCartByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}