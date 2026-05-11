using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Entities;

[ExcludeFromCodeCoverage]
public class TradeOffer : BaseEntity
{
    public Guid UserId { get; set; }
    public TradeOfferStatus Status { get; set; }

    public User User { get; set; } = null!;
    public ICollection<TradeOfferItem> Items { get; set; } = [];
}
