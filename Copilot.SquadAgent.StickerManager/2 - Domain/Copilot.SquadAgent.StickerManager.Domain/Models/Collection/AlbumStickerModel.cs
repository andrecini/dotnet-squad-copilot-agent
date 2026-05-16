using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class AlbumStickerModel
{
    public Guid StickerId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string TeamCode { get; set; } = string.Empty;
    public StickerRarity Rarity { get; set; }
    public bool Owned { get; set; }
    public int QuantityOwned { get; set; }
    public int QuantityDuplicate { get; set; }
}
