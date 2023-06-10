namespace Application.Commons.Types;

public static class CacheKey
{
    public static string GetShoppingCartKey(Guid userId)
    {
        return $"shopping_cart_{userId}";
    }

    public static string GetProductKey(Guid productId)
    {
        return $"product_{productId}";
    }
}