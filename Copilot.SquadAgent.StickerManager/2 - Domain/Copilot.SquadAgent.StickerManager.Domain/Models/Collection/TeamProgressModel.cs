namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

public class TeamProgressModel
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int TotalStickers { get; set; }
    public int OwnedCount { get; set; }
    public double CompletionPercentage { get; set; }
}
