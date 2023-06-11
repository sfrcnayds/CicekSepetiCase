using Application.Abstractions.Data;
using Domain.Abstractions.Repositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IApplicationDbContext _context;


    public UserRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsUserExistAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
        return user is not null;
    }
}