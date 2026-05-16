using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class AlbumQueryRequestMock
{
    public static AlbumQueryRequest Default() => new()
    {
        Page       = 1,
        PageSize   = 20,
        SortByTeam = false
    };

    public static AlbumQueryRequest WithSortByTeam() => new()
    {
        Page       = 1,
        PageSize   = 20,
        SortByTeam = true
    };

    public static AlbumQueryRequest WithPagination(int page, int pageSize) => new()
    {
        Page       = page,
        PageSize   = pageSize,
        SortByTeam = false
    };

    public static AlbumQueryRequest WithTeamFilter(Guid teamId) => new()
    {
        Page       = 1,
        PageSize   = 20,
        SortByTeam = false,
        TeamId     = teamId
    };

    public static AlbumQueryRequest WithTeamFilterAndPagination(Guid teamId, int page, int pageSize) => new()
    {
        Page       = page,
        PageSize   = pageSize,
        SortByTeam = false,
        TeamId     = teamId
    };

    public static AlbumQueryRequest WithTeamFilterAndSort(Guid teamId) => new()
    {
        Page       = 1,
        PageSize   = 20,
        SortByTeam = true,
        TeamId     = teamId
    };
}
