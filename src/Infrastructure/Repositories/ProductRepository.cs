using Domain.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid productId,CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);
    }
}