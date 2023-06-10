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
    public async Task AddAsync(ShoppingCartItem shoppingCartItem,CancellationToken cancellationToken = default)
    {
        await _context.ShoppingCartItems.AddAsync(shoppingCartItem,cancellationToken);
    }

    public void Update(ShoppingCartItem shoppingCartItem)
    {
        _context.Attach(shoppingCartItem);
        _context.Entry(shoppingCartItem).State = EntityState.Modified;
    }
}