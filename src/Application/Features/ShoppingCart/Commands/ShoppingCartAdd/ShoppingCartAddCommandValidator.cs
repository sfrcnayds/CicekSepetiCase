using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.Features.ShoppingCart.Commands.ShoppingCartAdd;

public class ShoppingCartAddCommandValidator : AbstractValidator<ShoppingCartAddCommand>
{
    private readonly IProductRepository _productRepository;

    public ShoppingCartAddCommandValidator(IUserRepository userRepository, IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.ShoppingCartAddRequest.UserId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("User Id can't be null")
            .MustAsync(userRepository.IsUserExistAsync)
            .WithMessage("User not found with given Id.");

        RuleFor(x => x.ShoppingCartAddRequest.ShoppingCardAddProduct.ProductId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("Product Id can't be null.")
            .MustAsync(IsProductExist)
            .WithMessage("Product not found with given Id.");

        RuleFor(x => x.ShoppingCartAddRequest.ShoppingCardAddProduct.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }

    private async Task<bool> IsProductExist(Guid productId, CancellationToken cancellationToken = default) =>
        await _productRepository.GetByIdAsync(productId, cancellationToken) != null;
}