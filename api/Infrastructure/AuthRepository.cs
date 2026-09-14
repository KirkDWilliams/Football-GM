using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public class AuthRepository(AppDbContext context) : IAuthRepository
{
    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return context.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public Task<User?> GetUserByIdReadOnlyAsync(string userId, CancellationToken cancellationToken = default)
    {
        return context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshToken?> GetRefreshTokenByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return context.RefreshTokens
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
    }

    public Task<RefreshToken?> GetRefreshTokenWithUserByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return context.RefreshTokens
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
    }

    public void AddRefreshToken(RefreshToken token)
    {
        context.RefreshTokens.Add(token);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
