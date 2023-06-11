using Application.Commons.DTOs;
using Application.Features.ShoppingCart.Commands.ShoppingCartAdd;
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

    [HttpPost, Route("[action]")]
    public async Task<ActionResult> AddProduct(ShoppingCartAddRequest shoppingCartAddRequest,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new ShoppingCartAddCommand(shoppingCartAddRequest), cancellationToken);
        return NoContent();
    }
}