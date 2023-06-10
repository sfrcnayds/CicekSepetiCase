using Domain.Abstractions.Repositories;
using MediatR;

namespace Application.Features.ShoppingCart.Queries.ShoppingCartGetByUserId;

public class
    ShoppingCartGetByUserIdQueryHandler : IRequestHandler<ShoppingCartGetByUserIdQuery, Domain.Entities.ShoppingCart?>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public ShoppingCartGetByUserIdQueryHandler(IShoppingCartRepository shoppingCartRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
    }

    public async Task<Domain.Entities.ShoppingCart?> Handle(ShoppingCartGetByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _shoppingCartRepository.GetShoppingCartByUserIdAsync(request.UserId, cancellationToken);
    }
}