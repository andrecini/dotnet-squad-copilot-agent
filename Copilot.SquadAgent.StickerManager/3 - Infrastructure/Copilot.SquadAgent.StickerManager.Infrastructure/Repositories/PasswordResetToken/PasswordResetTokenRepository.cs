using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PasswordResetTokenEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.PasswordResetToken;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.PasswordResetToken;

public class PasswordResetTokenRepository(AppDbContext dbContext) : IPasswordResetTokenRepository
{
    public async Task<Result<PasswordResetTokenEntity>> CreateAsync(PasswordResetTokenEntity token, CancellationToken cancellationToken)
    {
        await dbContext.PasswordResetTokens.AddAsync(token, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<PasswordResetTokenEntity>.Success(token);
    }

    public async Task<Result<PasswordResetTokenEntity>> GetValidByTokenAsync(string token, CancellationToken cancellationToken)
    {
        var resetToken = await dbContext.PasswordResetTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Token == token
                     && !x.IsUsed
                     && x.ExpiresAt > DateTime.UtcNow
                     && x.DeletedAt == null,
                cancellationToken);

        if (resetToken is null)
            return Result<PasswordResetTokenEntity>.Failure(ResultCode.NotFound, "Token não encontrado ou expirado.", statusCode: 404);

        return Result<PasswordResetTokenEntity>.Success(resetToken);
    }

    public async Task<Result<PasswordResetTokenEntity>> MarkAsUsedAsync(PasswordResetTokenEntity token, CancellationToken cancellationToken)
    {
        var tracked = await dbContext.PasswordResetTokens
            .FirstOrDefaultAsync(x => x.Id == token.Id, cancellationToken);

        if (tracked is null)
            return Result<PasswordResetTokenEntity>.Failure(ResultCode.NotFound, "Token não encontrado.", statusCode: 404);

        tracked.IsUsed = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<PasswordResetTokenEntity>.Success(tracked);
    }
}
