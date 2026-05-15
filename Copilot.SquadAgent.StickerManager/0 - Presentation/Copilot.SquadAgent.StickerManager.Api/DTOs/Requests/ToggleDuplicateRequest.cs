using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Copilot.SquadAgent.StickerManager.Domain.Enums;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

[ExcludeFromCodeCoverage]
public class ToggleDuplicateRequest
{
    [JsonPropertyName("action")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DuplicateAction Action { get; set; }
}
