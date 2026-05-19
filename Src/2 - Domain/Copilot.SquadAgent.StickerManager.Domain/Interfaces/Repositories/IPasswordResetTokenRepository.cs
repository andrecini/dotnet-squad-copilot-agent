using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;

public interface IPasswordResetTokenRepository
{
    Task<Result<PasswordResetToken>> CreateAsync(PasswordResetToken token, CancellationToken cancellationToken);
    Task<Result<PasswordResetToken>> GetValidByTokenAsync(string token, CancellationToken cancellationToken);
    Task<Result<PasswordResetToken>> MarkAsUsedAsync(PasswordResetToken token, CancellationToken cancellationToken);
}
