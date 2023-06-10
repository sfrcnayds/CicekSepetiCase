using Application.Abstractions.Caching;
using Application.Commons.Types;
using Domain.Abstractions.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class CachedShoppingCartRepository : IShoppingCartRepository
{
    private readonly ShoppingCartRepository _decorator;
    private readonly ICacheService _cacheService;

    public CachedShoppingCartRepository(ShoppingCartRepository decorator, ICacheService cacheService)
    {
        _decorator = decorator;
        _cacheService = cacheService;
    }

    public async Task AddAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken)
    {
        await _decorator.AddAsync(shoppingCart, cancellationToken);
    }

    public async Task<ShoppingCart?> GetShoppingCartByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var key = CacheKey.GetShoppingCartKey(userId);
        var cachedUserShoppingCart = await _cacheService.GetAsync<ShoppingCart>(key, cancellationToken);
        if (cachedUserShoppingCart is not null) return cachedUserShoppingCart;

        var dbUserShoppingCart = await _decorator.GetShoppingCartByUserIdAsync(userId, cancellationToken);
        if (dbUserShoppingCart is null) return null;

        await _cacheService.SetAsync(key, dbUserShoppingCart, cancellationToken);
        return dbUserShoppingCart;
    }
}