using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class CollectionItemModelMock
{
    public static CollectionItemModel Valid() => new()
    {
        StickerId         = Guid.NewGuid(),
        PlayerName        = "Neymar Jr",
        TeamName          = "Brasil",
        TeamCode          = "BRA",
        Rarity            = StickerRarity.Base,
        QuantityOwned     = 1,
        QuantityDuplicate = 0,
        AcquiredAt        = DateTime.UtcNow
    };

    public static IReadOnlyList<CollectionItemModel> List(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => new CollectionItemModel
            {
                StickerId         = Guid.NewGuid(),
                PlayerName        = $"Player {i}",
                TeamName          = "Brasil",
                TeamCode          = "BRA",
                Rarity            = StickerRarity.Base,
                QuantityOwned     = 1,
                QuantityDuplicate = 0,
                AcquiredAt        = DateTime.UtcNow
            })
            .ToList();
    }

    public static PagedResult<CollectionItemModel> Paged(int count = 3, int page = 1, int pageSize = 20)
    {
        var items = List(count);
        return new PagedResult<CollectionItemModel>
        {
            Items      = items,
            TotalCount = items.Count,
            Page       = page,
            PageSize   = pageSize
        };
    }

    public static PagedResult<CollectionItemModel> PagedEmpty() => new()
    {
        Items      = [],
        TotalCount = 0,
        Page       = 1,
        PageSize   = 20
    };
}
