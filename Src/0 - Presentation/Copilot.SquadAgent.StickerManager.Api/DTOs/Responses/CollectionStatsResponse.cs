using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;

[ExcludeFromCodeCoverage]
public class CollectionStatsResponse
{
    public int TotalOwned { get; set; }
    public int TotalMissing { get; set; }
    public int TotalStickers { get; set; }
    public double CompletionPercentage { get; set; }
    public int Duplicates { get; set; }
    public IReadOnlyList<TeamStatsResponse> ByTeam { get; set; } = [];
}
