using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;

[ExcludeFromCodeCoverage]
public class MissingStickerItemResponse
{
    public Guid StickerId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public StickerRarity Rarity { get; set; }
}
