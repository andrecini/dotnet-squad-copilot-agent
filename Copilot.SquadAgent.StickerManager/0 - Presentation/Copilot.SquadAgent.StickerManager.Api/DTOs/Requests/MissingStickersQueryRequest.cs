using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

[ExcludeFromCodeCoverage]
public class MissingStickersQueryRequest
{
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 100;
}
