using Application.Abstractions.Data;
using Domain.Entities;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Context;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public virtual DbSet<Product> Products { get; set; } = null!;
    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
    public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    private readonly ILogger<ApplicationDbContext> _logger;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger) : base(options)
    {
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new ())
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    break;
            }
            
            _logger.LogInformation("Entity [{State}]: {EntityName} - GUID: {Guid}", entry.State, entry.GetType().Name, entry.Entity.Id);
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}