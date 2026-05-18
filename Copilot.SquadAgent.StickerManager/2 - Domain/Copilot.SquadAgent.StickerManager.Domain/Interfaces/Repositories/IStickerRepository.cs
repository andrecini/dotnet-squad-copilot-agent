using Copilot.SquadAgent.StickerManager.Domain.Entities;
using Copilot.SquadAgent.StickerManager.Domain.Models;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;

public interface IStickerRepository
{
    Task<Result<Sticker>> GetByIdAsync(Guid stickerId, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<MissingStickerItemModel>>> ListMissingByUserAsync(MissingStickersModel filter, CancellationToken cancellationToken);
    Task<Result<PagedResult<AlbumStickerModel>>> GetAlbumAsync(AlbumQueryModel query, CancellationToken cancellationToken);
    Task<Result<TeamProgressModel>> GetTeamProgressAsync(Guid teamId, Guid userId, CancellationToken cancellationToken);
}
