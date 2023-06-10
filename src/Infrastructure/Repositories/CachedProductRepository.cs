using Application.Abstractions.Caching;
using Application.Commons.Types;
using Domain.Abstractions.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class CachedProductRepository : IProductRepository
{
    private readonly ProductRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedProductRepository(ProductRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var key = CacheKey.GetProductKey(productId);
        var cacheProduct = await _cacheService.GetAsync<Product>(key, cancellationToken);
        if (cacheProduct is not null) return cacheProduct;
        
        var dbProduct = await _decorated.GetByIdAsync(productId, cancellationToken);
        if (dbProduct is null)
        {
            return null;
        }
        await _cacheService.SetAsync(key, dbProduct, cancellationToken);
        return dbProduct;
    }
}