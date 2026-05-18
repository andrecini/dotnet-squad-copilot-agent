using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class MissingStickersQueryRequestMock
{
    public static MissingStickersQueryRequest Valid() => new()
    {
        Page = 1,
        PageSize = 100
    };

    public static MissingStickersQueryRequest WithSort(string sort = "team") => new()
    {
        Sort = sort,
        Page = 1,
        PageSize = 100
    };

    public static MissingStickersQueryRequest WithPagination(int page, int pageSize) => new()
    {
        Page = page,
        PageSize = pageSize
    };

    public static MissingStickersQueryRequest WithPageZero() => new()
    {
        Page = 0,
        PageSize = 100,
        Sort = null
    };

    public static MissingStickersQueryRequest WithPageSizeZero() => new()
    {
        Page = 1,
        PageSize = 0,
        Sort = null
    };

    public static MissingStickersQueryRequest WithPageSizeAboveMaximum() => new()
    {
        Page = 1,
        PageSize = 501,
        Sort = null
    };

    public static MissingStickersQueryRequest WithInvalidSort() => new()
    {
        Page = 1,
        PageSize = 100,
        Sort = "invalid_sort"
    };

    public static MissingStickersQueryRequest WithNullSort() => new()
    {
        Page = 1,
        PageSize = 100,
        Sort = null
    };
}
