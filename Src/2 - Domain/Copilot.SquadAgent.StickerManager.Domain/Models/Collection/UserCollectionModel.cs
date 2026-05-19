using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class UserCollectionModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StickerId { get; set; }
    public int QuantityOwned { get; set; }
    public int QuantityDuplicate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
