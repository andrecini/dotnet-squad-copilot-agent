using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class MissingStickersModel
{
    public Guid UserId { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 100;
}
