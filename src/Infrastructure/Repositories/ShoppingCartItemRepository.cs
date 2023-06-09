using Domain.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShoppingCartItemRepository : IShoppingCartItemRepository
{
    private readonly ApplicationDbContext _context;

    public ShoppingCartItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingCartItem?> GetByIdAsync(Guid shoppingCartItemId)
    {
        return await _context.ShoppingCartItems.FirstOrDefaultAsync(item => item.Id == shoppingCartItemId);
    }

    public async Task AddAsync(ShoppingCartItem shoppingCartItem)
    {
        await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
    }

    public async Task AddRangeAsync(IEnumerable<ShoppingCartItem> shoppingCartItems)
    {
        await _context.AddRangeAsync(shoppingCartItems);
    }
}