using Application.Commons.Types;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly ApplicationDbContext _context;

    public ShoppingCartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
    {
        await _context.ShoppingCarts.AddAsync(shoppingCart, cancellationToken);
    }

    public async Task<ShoppingCart?> GetShoppingCartByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ShoppingCarts
            .Where(cart =>
                cart.UserId == userId &&
                cart.Name == ShoppingCartNameTypes.ShoppingCart)
            .Include(cart => cart.ShoppingCartItems)
            .ThenInclude(items => items.Product)
            .Select(cart => new ShoppingCart
            {
                Id = cart.Id,
                Name = cart.Name,
                UserId = cart.UserId,
                ShoppingCartItems = cart.ShoppingCartItems.Select(item => new ShoppingCartItem
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Product = new Product
                    {
                        Id = item.Product.Id,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        Sku = item.Product.Sku,
                        StockQuantity = item.Product.StockQuantity,
                        CreatedAt = item.Product.CreatedAt,
                        ModifiedAt = item.Product.ModifiedAt,
                        DeletedAt = item.Product.DeletedAt
                    },
                    ShoppingCartId = item.ShoppingCartId,
                    Quantity = item.Quantity,
                    CreatedAt = item.CreatedAt,
                    ModifiedAt = item.ModifiedAt,
                    DeletedAt = item.DeletedAt
                }).ToList()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}