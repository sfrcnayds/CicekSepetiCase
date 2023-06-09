using Domain.Primitives;

namespace Domain.Entities;

public class ShoppingCartItem : Entity
{
    public ShoppingCartItem()
    {
    }

    public ShoppingCartItem(Guid shoppingCartId, Guid productId, int quantity)
    {
        ShoppingCartId = shoppingCartId;
        ProductId = productId;
        Quantity = quantity;
    }

    public Guid ShoppingCartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public virtual Product Product { get; set; }
    public virtual ShoppingCart ShoppingCart { get; set; }
}