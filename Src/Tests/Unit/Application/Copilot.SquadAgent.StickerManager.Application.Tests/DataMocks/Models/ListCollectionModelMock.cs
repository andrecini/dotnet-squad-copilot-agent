using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class ListCollectionModelMock
{
    public static ListCollectionModel Valid(Guid? userId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Page = 1,
        PageSize = 100
    };

    public static ListCollectionModel WithTeamFilter(Guid? userId = null, Guid? teamId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        TeamId = teamId ?? Guid.NewGuid(),
        Page = 1,
        PageSize = 100
    };

    public static ListCollectionModel WithRarityFilter(Guid? userId = null, StickerRarity rarity = StickerRarity.Foil) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Rarity = rarity,
        Page = 1,
        PageSize = 100
    };

    public static ListCollectionModel WithPagination(Guid? userId = null, int page = 2, int limit = 10) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Page = page,
        PageSize = limit
    };

    public static ListCollectionModel WithSort(Guid? userId = null, string sort = "player_name") => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Sort = sort,
        Page = 1,
        PageSize = 100
    };
}
