using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Entities;

[ExcludeFromCodeCoverage]
public class TradeOfferItem : BaseEntity
{
    public Guid TradeOfferId { get; set; }
    public Guid StickerId { get; set; }
    public TradeOfferItemDirection Direction { get; set; }

    public TradeOffer TradeOffer { get; set; } = null!;
    public Sticker Sticker { get; set; } = null!;
}
