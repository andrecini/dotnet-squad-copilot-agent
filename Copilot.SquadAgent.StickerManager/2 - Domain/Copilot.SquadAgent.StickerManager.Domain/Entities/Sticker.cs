using Copilot.SquadAgent.StickerManager.Domain.Enums;

namespace Copilot.SquadAgent.StickerManager.Domain.Entities;

public class Sticker : BaseEntity
{
    public int Number { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public StickerRarity Rarity { get; set; }
    public Guid TeamId { get; set; }

    public Team Team { get; set; } = null!;
    public ICollection<UserCollection> UserCollections { get; set; } = [];
    public ICollection<TradeOfferItem> TradeOfferItems { get; set; } = [];
}
