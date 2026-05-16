using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;

public interface IStickerRepository
{
    Task<Result<Sticker>> GetByIdAsync(Guid stickerId, CancellationToken cancellationToken);
    Task<Result<Sticker?>> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<MissingStickerItemModel>>> ListMissingByUserAsync(MissingStickersModel filter, CancellationToken cancellationToken);
}
