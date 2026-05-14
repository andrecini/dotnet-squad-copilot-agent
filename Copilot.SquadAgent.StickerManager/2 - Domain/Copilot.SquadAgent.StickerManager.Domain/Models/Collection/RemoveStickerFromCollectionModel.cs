using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class RemoveStickerFromCollectionModel
{
    public Guid CollectionId { get; set; }
    public Guid UserId { get; set; }
}
