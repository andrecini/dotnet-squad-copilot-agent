using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class CollectionItemModel
{
    public Guid StickerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string TeamCode { get; set; } = string.Empty;
    public StickerRarity Rarity { get; set; }
    public int QuantityOwned { get; set; }
    public int QuantityDuplicate { get; set; }
    public DateTime AcquiredAt { get; set; }
}
