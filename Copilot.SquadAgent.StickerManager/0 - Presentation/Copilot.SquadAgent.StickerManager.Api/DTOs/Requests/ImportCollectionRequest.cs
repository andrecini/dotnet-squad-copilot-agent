using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

[ExcludeFromCodeCoverage]
public class ImportCollectionRequest
{
    public IFormFile File { get; set; } = null!;
}
