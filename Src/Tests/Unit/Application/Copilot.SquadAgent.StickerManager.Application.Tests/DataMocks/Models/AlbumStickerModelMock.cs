using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
using Copilot.SquadAgent.StickerManager.Domain.Result;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class AlbumStickerModelMock
{
    public static AlbumStickerModel OwnedSticker() => new()
    {
        StickerId         = Guid.NewGuid(),
        Code              = "BRA-001",
        PlayerName        = "Neymar Jr",
        TeamName          = "Brasil",
        TeamCode          = "BRA",
        Rarity            = StickerRarity.Base,
        Owned             = true,
        QuantityOwned     = 1,
        QuantityDuplicate = 0
    };

    public static AlbumStickerModel MissingSticker() => new()
    {
        StickerId         = Guid.NewGuid(),
        Code              = "ARG-001",
        PlayerName        = "Lionel Messi",
        TeamName          = "Argentina",
        TeamCode          = "ARG",
        Rarity            = StickerRarity.Base,
        Owned             = false,
        QuantityOwned     = 0,
        QuantityDuplicate = 0
    };

    public static IReadOnlyList<AlbumStickerModel> List(int ownedCount = 2, int missingCount = 3)
    {
        var owned = Enumerable.Range(1, ownedCount)
            .Select(i => new AlbumStickerModel
            {
                StickerId         = Guid.NewGuid(),
                Code              = $"BRA-{i:000}",
                PlayerName        = $"Player {i}",
                TeamName          = "Brasil",
                TeamCode          = "BRA",
                Rarity            = StickerRarity.Base,
                Owned             = true,
                QuantityOwned     = 1,
                QuantityDuplicate = 0
            });

        var missing = Enumerable.Range(1, missingCount)
            .Select(i => new AlbumStickerModel
            {
                StickerId         = Guid.NewGuid(),
                Code              = $"ARG-{i:000}",
                PlayerName        = $"Player {i}",
                TeamName          = "Argentina",
                TeamCode          = "ARG",
                Rarity            = StickerRarity.Base,
                Owned             = false,
                QuantityOwned     = 0,
                QuantityDuplicate = 0
            });

        return owned.Concat(missing).ToList();
    }

    public static IReadOnlyList<AlbumStickerModel> Empty() => [];

    public static PagedResult<AlbumStickerModel> Paged(int ownedCount = 2, int missingCount = 3, int page = 1, int pageSize = 20)
    {
        var items = List(ownedCount, missingCount);
        return new PagedResult<AlbumStickerModel>
        {
            Items      = items,
            TotalCount = items.Count,
            Page       = page,
            PageSize   = pageSize
        };
    }

    public static PagedResult<AlbumStickerModel> PagedEmpty() => new()
    {
        Items      = [],
        TotalCount = 0,
        Page       = 1,
        PageSize   = 20
    };
}
