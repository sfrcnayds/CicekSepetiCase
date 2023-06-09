using Domain.Primitives;

namespace Domain.Entities;

public class ShoppingCart : Entity
{
    public Guid UserId;

    public ShoppingCart()
    {
    }

    public ShoppingCart(Guid id, Guid userId, string name)
    {
        Id = id;
        UserId = userId;
        Name = name;
    }

    public string Name { get; set; }

    public virtual User User { get; set; }
    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
}