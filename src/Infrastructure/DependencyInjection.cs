using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Domain.Abstractions.Repositories;
using Infrastructure.Caching;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = Environment.GetEnvironmentVariable("RedisConnection") ??
                              configuration.GetConnectionString("Redis");

        var postgresConnection = Environment.GetEnvironmentVariable("PostgresConnection") ??
                                 configuration.GetConnectionString("PostgresConnection");
        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(postgresConnection,
                    optionsBuilder =>
                    {
                        optionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }
                )
        );
        // Add Redis
        services.AddStackExchangeRedisCache(redisOptions => { redisOptions.Configuration = redisConnection; });

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        services.AddScoped<ProductRepository>();
        services.AddScoped<IProductRepository, CachedProductRepository>();

        services.AddScoped<ShoppingCartRepository>();
        services.AddScoped<IShoppingCartRepository, CachedShoppingCartRepository>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IShoppingCartItemRepository, ShoppingCartItemRepository>();

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}