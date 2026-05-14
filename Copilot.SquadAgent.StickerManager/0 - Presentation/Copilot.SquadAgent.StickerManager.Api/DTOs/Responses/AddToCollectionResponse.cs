using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;

[ExcludeFromCodeCoverage]
public class AddToCollectionResponse
{
    [JsonPropertyName("collection_id")]
    public Guid CollectionId { get; set; }

    [JsonPropertyName("sticker_id")]
    public Guid StickerId { get; set; }

    [JsonPropertyName("quantity_owned")]
    public int QuantityOwned { get; set; }

    [JsonPropertyName("quantity_duplicate")]
    public int QuantityDuplicate { get; set; }
}
