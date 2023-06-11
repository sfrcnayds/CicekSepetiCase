namespace Application.Commons.DTOs;

public sealed record ShoppingCartAddRequest(Guid UserId, ShoppingCardAddProductDto ShoppingCardAddProduct);