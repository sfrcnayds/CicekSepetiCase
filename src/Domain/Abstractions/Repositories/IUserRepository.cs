namespace Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<bool> IsUserExistAsync(Guid userId);
}