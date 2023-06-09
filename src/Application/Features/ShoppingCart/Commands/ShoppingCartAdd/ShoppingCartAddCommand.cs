using Application.Commons.DTOs;
using MediatR;

namespace Application.Features.ShoppingCart.Commands.ShoppingCartAdd;

public sealed record ShoppingCartAddCommand(ShoppingCartAddRequest ShoppingCartAddRequest) : IRequest;