using System.Text.Json.Serialization;
using Domain.Primitives;

namespace Domain.Entities;

public class User : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;
    
    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
}