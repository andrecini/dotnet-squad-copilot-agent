using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;

public static class MissingStickerItemModelMock
{
    public static MissingStickerItemModel Valid() => new()
    {
        StickerId  = Guid.NewGuid(),
        Code       = "BRA001",
        PlayerName = "Neymar Jr",
        TeamName   = "Brasil",
        TeamCode   = "BRA",
        Rarity     = StickerRarity.Base
    };

    public static IReadOnlyList<MissingStickerItemModel> List(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => new MissingStickerItemModel
            {
                StickerId  = Guid.NewGuid(),
                Code       = $"BRA{i:D3}",
                PlayerName = $"Player {i}",
                TeamName   = "Brasil",
                TeamCode   = "BRA",
                Rarity     = StickerRarity.Base
            })
            .ToList();
    }

    public static PagedResult<MissingStickerItemModel> Paged(int count = 3, int page = 1, int pageSize = 20)
    {
        var items = List(count);
        return new PagedResult<MissingStickerItemModel>
        {
            Items      = items,
            TotalCount = items.Count,
            Page       = page,
            PageSize   = pageSize
        };
    }

    public static PagedResult<MissingStickerItemModel> PagedEmpty() => new()
    {
        Items      = [],
        TotalCount = 0,
        Page       = 1,
        PageSize   = 20
    };
}
