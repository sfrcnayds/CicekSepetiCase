using Application.Abstractions.Data;
using Application.Commons.Types;
using Application.Exceptions;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.ShoppingCart.Commands.ShoppingCartAdd;

public class ShoppingCartAddCommandHandler : IRequestHandler<ShoppingCartAddCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IShoppingCartItemRepository _shoppingCartItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingCartAddCommandHandler(
        IProductRepository productRepository,
        IShoppingCartRepository shoppingCartRepository,
        IShoppingCartItemRepository shoppingCartItemRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _shoppingCartItemRepository = shoppingCartItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ShoppingCartAddCommand request, CancellationToken cancellationToken)
    {
        var shoppingCartAddRequest = request.ShoppingCartAddRequest;
        var userShoppingCart =
            await _shoppingCartRepository.GetShoppingCartByUserIdAsync(shoppingCartAddRequest.UserId);
        var product = await _productRepository.GetByIdAsync(request.ShoppingCartAddRequest.Product.ProductId);
        
        if (product is null)
        {
            throw new NotFoundException(nameof(Product),request.ShoppingCartAddRequest.Product.ProductId);
        }

        if (product.StockQuantity < request.ShoppingCartAddRequest.Product.Quantity)
        {
            throw new Exception("Product is out of stock.");
        }
        
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            Guid shoppingCartId;
            var isExistingItem = false;
            
            if (userShoppingCart is null)
            {
                shoppingCartId = Guid.NewGuid();
                await _shoppingCartRepository.AddAsync(
                    new Domain.Entities.ShoppingCart(shoppingCartId,
                        shoppingCartAddRequest.UserId,
                        ShoppingCartNameTypes.ShoppingCart)
                );
            }
            else
            {
                shoppingCartId = userShoppingCart.Id;
                var productId = shoppingCartAddRequest.Product.ProductId;
                var quantity = shoppingCartAddRequest.Product.Quantity;
                var userShoppingCartExistItem =
                    userShoppingCart.ShoppingCartItems.FirstOrDefault(item => item.ProductId == productId);
                if (userShoppingCartExistItem is not null)
                {
                    if (userShoppingCartExistItem.Quantity + quantity > product.StockQuantity)
                    {
                        throw new Exception("Product is out of stock.");
                    }
                    var shoppingCartItem = await _shoppingCartItemRepository.GetByIdAsync(userShoppingCartExistItem.Id);
                    shoppingCartItem!.Quantity += quantity;
                    isExistingItem = true;
                }
            }

            if (!isExistingItem)
            {
                await _shoppingCartItemRepository
                    .AddAsync(
                        new ShoppingCartItem(shoppingCartId,
                            shoppingCartAddRequest.Product.ProductId,
                            shoppingCartAddRequest.Product.Quantity)
                    );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}