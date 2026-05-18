using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries;

[ExcludeFromCodeCoverage]
public class MissingStickersQueryRequest : BasePaginatedQueryRequest
{
    public string? Sort { get; set; }
}
