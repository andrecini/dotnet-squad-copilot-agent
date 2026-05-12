using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

[ExcludeFromCodeCoverage]
public class UpdateUserProfileRequest
{
    public string? Email { get; set; }
    public string? Name { get; set; }
}
