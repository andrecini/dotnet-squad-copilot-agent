using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses.Paged;

[ExcludeFromCodeCoverage]
public class PagedAlbumResponse : BasePagedResponse
{
    public IReadOnlyList<AlbumItemResponse> Items { get; set; } = [];
}
