using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Domain.Enums;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class CollectionQueryRequestMock
{
    public static CollectionQueryRequest Valid() => new()
    {
        Page = 1,
        Limit = 100
    };

    public static CollectionQueryRequest WithTeamFilter(Guid teamId) => new()
    {
        TeamId = teamId,
        Page = 1,
        Limit = 100
    };

    public static CollectionQueryRequest WithRarityFilter(StickerRarity rarity = StickerRarity.Foil) => new()
    {
        Rarity = rarity,
        Page = 1,
        Limit = 100
    };

    public static CollectionQueryRequest WithSort(string sort = "player_name") => new()
    {
        Sort = sort,
        Page = 1,
        Limit = 100
    };

    public static CollectionQueryRequest WithPageZero() => new()
    {
        Page = 0,
        Limit = 100,
        Sort = null
    };

    public static CollectionQueryRequest WithLimitZero() => new()
    {
        Page = 1,
        Limit = 0,
        Sort = null
    };

    public static CollectionQueryRequest WithLimitAboveMaximum() => new()
    {
        Page = 1,
        Limit = 501,
        Sort = null
    };

    public static CollectionQueryRequest WithInvalidSort() => new()
    {
        Page = 1,
        Limit = 100,
        Sort = "invalid_sort"
    };

    public static CollectionQueryRequest WithNullSort() => new()
    {
        Page = 1,
        Limit = 100,
        Sort = null
    };
}
