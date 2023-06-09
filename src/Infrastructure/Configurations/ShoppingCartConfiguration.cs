using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.ModifiedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .HasOne(e => e.User)
            .WithMany(e => e.ShoppingCarts)
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("shopping_cart_user_id_fk");
    }
}