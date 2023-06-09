using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public virtual DbSet<Product> Products { get; set; } = null!;
    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
    public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}