using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries;

[ExcludeFromCodeCoverage]
public class AlbumQueryRequest : BasePaginatedQueryRequest
{
    public bool? SortByTeam { get; set; } = false;
    public Guid? TeamId { get; set; } = null;
}
