using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid productId);
}