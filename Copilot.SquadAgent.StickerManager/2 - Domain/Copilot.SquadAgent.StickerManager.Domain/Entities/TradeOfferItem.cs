using Copilot.SquadAgent.StickerManager.Domain.Enums;

namespace Copilot.SquadAgent.StickerManager.Domain.Entities;

public class TradeOfferItem : BaseEntity
{
    public Guid TradeOfferId { get; set; }
    public Guid StickerId { get; set; }
    public TradeOfferItemDirection Direction { get; set; }

    public TradeOffer TradeOffer { get; set; } = null!;
    public Sticker Sticker { get; set; } = null!;
}
