using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;

public interface IStickerRepository
{
    Task<Result<Sticker>> GetByIdAsync(Guid stickerId, CancellationToken cancellationToken);
}
