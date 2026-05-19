using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses.Paged;

[ExcludeFromCodeCoverage]
public class PagedCollectionResponse : BasePagedResponse
{
    public IReadOnlyList<CollectionItemResponse> Items { get; set; } = [];
}
