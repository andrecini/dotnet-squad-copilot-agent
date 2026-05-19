using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses.Paged;

[ExcludeFromCodeCoverage]
public class PagedMissingStickersResponse : BasePagedResponse
{
    public IReadOnlyList<MissingStickerItemResponse> Items { get; set; } = [];
}
