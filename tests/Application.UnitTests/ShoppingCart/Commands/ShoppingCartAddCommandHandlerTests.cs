using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Commons.DTOs;
using Application.Exceptions;
using Application.Features.ShoppingCart.Commands.ShoppingCartAdd;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.UnitTests.ShoppingCart.Commands;

public class ShoppingCartAddCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IShoppingCartRepository> _shoppingCartRepositoryMock;
    private readonly Mock<IShoppingCartItemRepository> _shoppingCartItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICacheService> _cacheServiceMock;

    public ShoppingCartAddCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _shoppingCartRepositoryMock = new Mock<IShoppingCartRepository>();
        _shoppingCartItemRepositoryMock = new Mock<IShoppingCartItemRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cacheServiceMock = new Mock<ICacheService>();
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var validator = new ShoppingCartAddCommandValidator(_userRepositoryMock.Object, _productRepositoryMock.Object);

        var command =
            new ShoppingCartAddCommand(new ShoppingCartAddRequest(Guid.NewGuid(),
                new ShoppingCardAddProductDto(Guid.NewGuid(), 1)));

        // Act && Assert
        var validationResult = await validator.ValidateAsync(command);
        Assert.Contains(validationResult.Errors, o => o.PropertyName == "ShoppingCartAddRequest.UserId");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Arrange
        var validator = new ShoppingCartAddCommandValidator(_userRepositoryMock.Object, _productRepositoryMock.Object);

        var command =
            new ShoppingCartAddCommand(new ShoppingCartAddRequest(Guid.NewGuid(),
                new ShoppingCardAddProductDto(Guid.NewGuid(), 1)));

        // Act && Assert
        var validationResult = await validator.ValidateAsync(command);
        Assert.Contains(validationResult.Errors,
            o => o.PropertyName == "ShoppingCartAddRequest.ShoppingCardAddProduct.ProductId");
    }

    [Fact]
    public async Task Handle_ShouldThrowInsufficientStockAvailableException_WhenStockQuantityIsNotEnough()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var productGuid = Guid.NewGuid();
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = productGuid, StockQuantity = 5 });

        var handler = new ShoppingCartAddCommandHandler(
            _productRepositoryMock.Object,
            _shoppingCartRepositoryMock.Object,
            _shoppingCartItemRepositoryMock.Object,
            _userRepositoryMock.Object,
            _cacheServiceMock.Object,
            _unitOfWorkMock.Object
        );

        var command =
            new ShoppingCartAddCommand(new ShoppingCartAddRequest(Guid.NewGuid(),
                new ShoppingCardAddProductDto(productGuid, 10)));

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientStockAvailableException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });
    }

    [Fact]
    public async Task
        Handle_ShouldThrowInsufficientStockAvailableException_WhenStockQuantityIsNotEnoughWithUserCartItem()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var productGuid = Guid.NewGuid();
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = productGuid, StockQuantity = 5 });
        _shoppingCartRepositoryMock
            .Setup(repo => repo.GetShoppingCartByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.ShoppingCart
            {
                Name = "SHOPPING_CART",
                ShoppingCartItems = new List<ShoppingCartItem>
                {
                    new(Guid.NewGuid(), productGuid, 3)
                }
            });

        var handler = new ShoppingCartAddCommandHandler(
            _productRepositoryMock.Object,
            _shoppingCartRepositoryMock.Object,
            _shoppingCartItemRepositoryMock.Object,
            _userRepositoryMock.Object,
            _cacheServiceMock.Object,
            _unitOfWorkMock.Object
        );

        var command =
            new ShoppingCartAddCommand(new ShoppingCartAddRequest(Guid.NewGuid(),
                new ShoppingCardAddProductDto(productGuid, 4)));

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientStockAvailableException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });
    }

    [Fact]
    public async Task Handle_ShouldAddNewShoppingCartItem_WhenUserDoesNotHaveShoppingCart()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);


        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = productId, StockQuantity = 10 });

        _shoppingCartRepositoryMock
            .Setup(repo => repo.GetShoppingCartByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.ShoppingCart?)null);


        var handler = new ShoppingCartAddCommandHandler(
            _productRepositoryMock.Object,
            _shoppingCartRepositoryMock.Object,
            _shoppingCartItemRepositoryMock.Object,
            _userRepositoryMock.Object,
            _cacheServiceMock.Object,
            _unitOfWorkMock.Object
        );

        var command =
            new ShoppingCartAddCommand(new ShoppingCartAddRequest(userId, new ShoppingCardAddProductDto(productId, 2)));

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _shoppingCartRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Domain.Entities.ShoppingCart>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _shoppingCartItemRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<ShoppingCartItem>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(cache => cache.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldAddNewShoppingCartItem_WhenUserShoppingCartDoesNotHaveExistingItem()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var productGuid = Guid.NewGuid();
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = productGuid, StockQuantity = 10 });

        var shoppingCartId = Guid.NewGuid();
        _shoppingCartRepositoryMock
            .Setup(repo => repo.GetShoppingCartByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.ShoppingCart
                { Id = shoppingCartId, Name = "SHOPPING_CART", ShoppingCartItems = new List<ShoppingCartItem>() });

        var userId = Guid.NewGuid();

        var handler = new ShoppingCartAddCommandHandler(
            _productRepositoryMock.Object,
            _shoppingCartRepositoryMock.Object,
            _shoppingCartItemRepositoryMock.Object,
            _userRepositoryMock.Object,
            _cacheServiceMock.Object,
            _unitOfWorkMock.Object
        );

        var command =
            new ShoppingCartAddCommand(
                new ShoppingCartAddRequest(userId, new ShoppingCardAddProductDto(productGuid, 2)));

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _shoppingCartRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Domain.Entities.ShoppingCart>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _shoppingCartRepositoryMock.Verify(
            repo => repo.GetShoppingCartByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _shoppingCartItemRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<ShoppingCartItem>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(cache => cache.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldAddExistingShoppingCartItem_WhenUserShoppingCartHasExistingItem()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var productGuid = Guid.NewGuid();
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = productGuid, StockQuantity = 10 });
        _shoppingCartRepositoryMock
            .Setup(repo => repo.GetShoppingCartByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.ShoppingCart
            {
                Name = "SHOPPING_CART",
                ShoppingCartItems = new List<ShoppingCartItem>
                {
                    new(Guid.NewGuid(), productGuid, 3)
                }
            });

        var handler = new ShoppingCartAddCommandHandler(
            _productRepositoryMock.Object,
            _shoppingCartRepositoryMock.Object,
            _shoppingCartItemRepositoryMock.Object,
            _userRepositoryMock.Object,
            _cacheServiceMock.Object,
            _unitOfWorkMock.Object
        );

        var command =
            new ShoppingCartAddCommand(new ShoppingCartAddRequest(Guid.NewGuid(),
                new ShoppingCardAddProductDto(productGuid, 4)));


        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _shoppingCartRepositoryMock.Verify(
            repo => repo.GetShoppingCartByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _shoppingCartRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Domain.Entities.ShoppingCart>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _shoppingCartItemRepositoryMock.Verify(
            repo => repo.Update(It.IsAny<ShoppingCartItem>()),
            Times.Once);
        _shoppingCartItemRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<ShoppingCartItem>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        _cacheServiceMock.Verify(cache => cache.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}