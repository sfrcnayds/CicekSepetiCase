using Application.Commons.DTOs;
using Application.Features.ShoppingCart.Commands.ShoppingCartAdd;
using Application.Features.ShoppingCart.Queries.ShoppingCartGetByUserId;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController, Route("[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShoppingCartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet, Route("[action]")]
    public async Task<ActionResult<ShoppingCart>> GetUserShoppingCart(Guid userId,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new ShoppingCartGetByUserIdQuery(userId), cancellationToken));
    }

    [HttpPost, Route("[action]")]
    public async Task<ActionResult> AddItem(ShoppingCartAddRequest shoppingCartAddRequest,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new ShoppingCartAddCommand(shoppingCartAddRequest), cancellationToken);
        return NoContent();
    }
}