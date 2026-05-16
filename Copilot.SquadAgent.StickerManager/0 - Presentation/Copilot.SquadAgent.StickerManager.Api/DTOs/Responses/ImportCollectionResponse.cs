using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;

[ExcludeFromCodeCoverage]
public class ImportCollectionResponse
{
    [JsonPropertyName("imported")]
    public int Imported { get; set; }

    [JsonPropertyName("failed")]
    public int Failed { get; set; }

    [JsonPropertyName("errors")]
    public IReadOnlyList<ImportCollectionErrorResponse> Errors { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public class ImportCollectionErrorResponse
{
    [JsonPropertyName("line")]
    public int Line { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;
}
