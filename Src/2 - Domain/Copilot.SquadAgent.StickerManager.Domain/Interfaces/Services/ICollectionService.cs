using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using ResultNs = Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Services;

public interface ICollectionService
{
    Task<ResultNs.Result<UserCollectionModel>> AddStickerAsync(AddToCollectionModel model, CancellationToken cancellationToken);
    Task<ResultNs.Result> RemoveStickerFromCollectionAsync(RemoveStickerFromCollectionModel model, CancellationToken cancellationToken);
    Task<ResultNs.Result<UserCollectionModel>> ToggleDuplicateAsync(ToggleDuplicateModel model, CancellationToken cancellationToken);
    Task<ResultNs.Result<PagedResult<CollectionItemModel>>> ListCollectionAsync(ListCollectionModel model, CancellationToken cancellationToken);
    Task<ResultNs.Result<PagedResult<MissingStickerItemModel>>> ListMissingStickersAsync(MissingStickersModel model, CancellationToken cancellationToken);
    Task<ResultNs.Result<CollectionStatsModel>> GetCollectionStatsAsync(Guid userId, CancellationToken cancellationToken);
    Task<ResultNs.Result<PagedResult<AlbumStickerModel>>> GetAlbumAsync(AlbumQueryModel query, CancellationToken cancellationToken);
    Task<ResultNs.Result<TeamProgressModel>> GetTeamProgressAsync(Guid teamId, Guid userId, CancellationToken cancellationToken);
}
