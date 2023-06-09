using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
{
    public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
    {
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.ModifiedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder
            .HasOne(e => e.Product)
            .WithMany(e => e.ShoppingCartItems)
            .HasForeignKey(e => e.ProductId)
            .HasConstraintName("shopping_cart_item_product_id_fk");
        
        builder
            .HasOne(e => e.ShoppingCart)
            .WithMany(e => e.ShoppingCartItems)
            .HasForeignKey(e => e.ShoppingCartId)
            .HasConstraintName("shopping_cart_item_shopping_cart_id_fk");
    }
}