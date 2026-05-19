using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;

[ExcludeFromCodeCoverage]
public class CollectionItemResponse
{
    public Guid StickerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public StickerRarity Rarity { get; set; }
    public int QuantityOwned { get; set; }
    public int QuantityDuplicate { get; set; }
}
