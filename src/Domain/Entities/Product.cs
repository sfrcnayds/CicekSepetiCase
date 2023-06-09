using Domain.Primitives;

namespace Domain.Entities;

public class Product : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    
    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
}