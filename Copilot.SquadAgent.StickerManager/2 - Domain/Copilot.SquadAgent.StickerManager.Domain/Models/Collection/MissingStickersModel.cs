using Copilot.SquadAgent.StickerManager.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

[ExcludeFromCodeCoverage]
public class MissingStickersModel
{
    public Guid UserId { get; set; }
    public string? Sort { get; set; }
}
