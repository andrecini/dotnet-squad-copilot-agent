@using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class ToggleDuplicateModel
{
    public Guid CollectionId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
}
