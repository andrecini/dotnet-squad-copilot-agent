using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class AddToCollectionModel
{
    public Guid UserId { get; set; }
    public Guid StickerId { get; set; }
}
