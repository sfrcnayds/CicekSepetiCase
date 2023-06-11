using System.ComponentModel.DataAnnotations;

namespace Domain.Primitives;

public abstract class Entity
{
    [Key]
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}