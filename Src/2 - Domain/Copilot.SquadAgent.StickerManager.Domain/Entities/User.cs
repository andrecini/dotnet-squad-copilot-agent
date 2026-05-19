using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Entities;

[ExcludeFromCodeCoverage]
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<UserCollection> UserCollections { get; set; } = [];
    public ICollection<TradeOffer> TradeOffers { get; set; } = [];
}
