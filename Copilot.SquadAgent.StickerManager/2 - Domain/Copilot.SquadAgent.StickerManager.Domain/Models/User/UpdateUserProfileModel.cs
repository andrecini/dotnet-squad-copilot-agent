using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Domain.Models.User;

[ExcludeFromCodeCoverage]
public class UpdateUserProfileModel
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
}
