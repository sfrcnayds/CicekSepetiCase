using MediatR;

namespace Application.Features.ShoppingCart.Queries.ShoppingCartGetByUserId;

public record ShoppingCartGetByUserIdQuery(Guid UserId) : IRequest<Domain.Entities.ShoppingCart?>;