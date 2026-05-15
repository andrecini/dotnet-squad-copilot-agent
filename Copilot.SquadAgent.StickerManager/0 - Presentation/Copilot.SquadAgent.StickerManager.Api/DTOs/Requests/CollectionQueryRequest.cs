using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

[ExcludeFromCodeCoverage]
public class CollectionQueryRequest
{
    public Guid? TeamId { get; set; }
    public StickerRarity? Rarity { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 100;
}
