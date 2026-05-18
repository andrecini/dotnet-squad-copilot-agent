using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.AppServices.Interfaces;

public interface ICollectionAppService
{
    Task<IResult> AddStickerAsync(Guid userId, AddToCollectionRequest request, CancellationToken cancellationToken);
    Task<IResult> RemoveStickerFromCollectionAsync(Guid userId, Guid collectionId, CancellationToken cancellationToken);
    Task<IResult> ToggleDuplicateAsync(Guid userId, Guid collectionId, ToggleDuplicateRequest request, CancellationToken cancellationToken);
    Task<IResult> ListCollectionAsync(Guid userId, CollectionQueryRequest query, CancellationToken cancellationToken);
    Task<IResult> ListMissingStickersAsync(Guid userId, MissingStickersQueryRequest query, CancellationToken cancellationToken);
    Task<IResult> GetCollectionStatsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IResult> GetAlbumAsync(Guid userId, AlbumQueryRequest query, CancellationToken cancellationToken);
    Task<IResult> GetTeamProgressAsync(Guid userId, Guid teamId, CancellationToken cancellationToken);
}
