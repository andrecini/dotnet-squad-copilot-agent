using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class CollectionStatsModel
{
    public int TotalOwned { get; set; }
    public int TotalMissing { get; set; }
    public int TotalStickers { get; set; }
    public double CompletionPercentage { get; set; }
    public int Duplicates { get; set; }
    public IReadOnlyList<TeamStatsModel> ByTeam { get; set; } = [];
}
