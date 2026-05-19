using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;

[ExcludeFromCodeCoverage]
public class TeamStatsResponse
{
    public Guid TeamId { get; set; }
    public string Team { get; set; } = string.Empty;
    public string TeamCode { get; set; } = string.Empty;
    public int Owned { get; set; }
    public int Total { get; set; }
    public double CompletionPercentage { get; set; }
}
