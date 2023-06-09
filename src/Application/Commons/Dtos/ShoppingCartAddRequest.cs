namespace Application.Commons.DTOs;

public sealed record ShoppingCartAddRequest(Guid UserId, ProductDto Product);