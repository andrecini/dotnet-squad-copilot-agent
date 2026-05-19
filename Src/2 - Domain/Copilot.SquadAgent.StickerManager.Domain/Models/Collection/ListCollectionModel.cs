using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class ListCollectionModel
{
    public Guid UserId { get; set; }
    public Guid? TeamId { get; set; }
    public StickerRarity? Rarity { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
