using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

[ExcludeFromCodeCoverage]
public class AddToCollectionRequest
{
    [JsonPropertyName("sticker_id")]
    public Guid StickerId { get; set; }
}
