using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries;

[ExcludeFromCodeCoverage]
public class CollectionQueryRequest : BasePaginatedQueryRequest
{
    public Guid? TeamId { get; set; }
    public StickerRarity? Rarity { get; set; }
    public string? Sort { get; set; }
}
