using Copilot.SquadAgent.StickerManager.Domain.Enums;

namespace Copilot.SquadAgent.StickerManager.Domain.Entities;

public class TradeOffer : BaseEntity
{
    public Guid UserId { get; set; }
    public TradeOfferStatus Status { get; set; }

    public User User { get; set; } = null!;
    public ICollection<TradeOfferItem> Items { get; set; } = [];
}
