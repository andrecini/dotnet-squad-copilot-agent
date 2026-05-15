using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class MissingStickersModelMock
{
    public static MissingStickersModel Valid(Guid? userId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Page = 1,
        Limit = 100
    };

    public static MissingStickersModel WithSort(Guid? userId = null, string sort = "rarity") => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Sort = sort,
        Page = 1,
        Limit = 100
    };

    public static MissingStickersModel WithTeamSort(Guid? userId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Sort = "team",
        Page = 1,
        Limit = 100
    };

    public static MissingStickersModel WithNumberSort(Guid? userId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Sort = "number",
        Page = 1,
        Limit = 100
    };

    public static MissingStickersModel WithPagination(int page, int limit, Guid? userId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Page = page,
        Limit = limit
    };
}
