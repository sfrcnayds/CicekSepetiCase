using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Commons.DTOs;
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
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingCartAddCommandHandler(
        IProductRepository productRepository,
        IShoppingCartRepository shoppingCartRepository,
        IShoppingCartItemRepository shoppingCartItemRepository,
        IUserRepository userRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork
    )
    {
        _productRepository = productRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _shoppingCartItemRepository = shoppingCartItemRepository;
        _userRepository = userRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ShoppingCartAddCommand request, CancellationToken cancellationToken)
    {
        var shoppingCartAddRequest = request.ShoppingCartAddRequest;

        var product = (
            await _productRepository.GetByIdAsync(
                request.ShoppingCartAddRequest.ShoppingCardAddProduct.ProductId,
                cancellationToken)
        )!;


        if (product.StockQuantity < request.ShoppingCartAddRequest.ShoppingCardAddProduct.Quantity)
        {
            throw new InsufficientStockAvailableException();
        }

        var userShoppingCart = await _shoppingCartRepository
            .GetShoppingCartByUserIdAsync(shoppingCartAddRequest.UserId, cancellationToken);

        var shoppingCartId = userShoppingCart?.Id ??
                             await AddShoppingCartToUser(shoppingCartAddRequest.UserId, cancellationToken);

        var userShoppingCartExistItem = userShoppingCart?.ShoppingCartItems
            .FirstOrDefault(item => item.ProductId == shoppingCartAddRequest.ShoppingCardAddProduct.ProductId);

        if (userShoppingCartExistItem is not null)
        {
            AddExistingShoppingCartItemToCart(shoppingCartAddRequest, userShoppingCartExistItem, product);
        }
        else
        {
            await AddShoppingCartItemToCart(shoppingCartId, shoppingCartAddRequest, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync(CacheKey.GetShoppingCartKey(shoppingCartAddRequest.UserId),
            cancellationToken);
    }

    private async Task<Guid> AddShoppingCartToUser(Guid userId, CancellationToken cancellationToken = default)
    {
        var shoppingCartId = Guid.NewGuid();

        var newShoppingCart =
            new Domain.Entities.ShoppingCart(shoppingCartId, userId, ShoppingCartNameTypes.ShoppingCart);

        await _shoppingCartRepository.AddAsync(newShoppingCart, cancellationToken);

        return shoppingCartId;
    }

    private void AddExistingShoppingCartItemToCart(ShoppingCartAddRequest shoppingCartAddRequest,
        ShoppingCartItem userShoppingCartExistItem, Product product)
    {
        var quantity = shoppingCartAddRequest.ShoppingCardAddProduct.Quantity;
        if (userShoppingCartExistItem.Quantity + quantity > product.StockQuantity)
        {
            throw new InsufficientStockAvailableException();
        }

        userShoppingCartExistItem.Quantity += quantity;
        _shoppingCartItemRepository.Update(userShoppingCartExistItem);
    }

    private async Task AddShoppingCartItemToCart(Guid shoppingCartId, ShoppingCartAddRequest shoppingCartAddRequest,
        CancellationToken cancellationToken = default)
    {
        var newShoppingCartItem = new ShoppingCartItem(shoppingCartId,
            shoppingCartAddRequest.ShoppingCardAddProduct.ProductId,
            shoppingCartAddRequest.ShoppingCardAddProduct.Quantity);

        await _shoppingCartItemRepository.AddAsync(newShoppingCartItem, cancellationToken);
    }
}