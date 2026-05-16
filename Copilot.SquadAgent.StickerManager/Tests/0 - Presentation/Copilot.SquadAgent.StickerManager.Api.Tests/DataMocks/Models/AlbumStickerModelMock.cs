using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;

public static class AlbumStickerModelMock
{
    public static PagedResult<AlbumStickerModel> Paged(int ownedCount = 2, int missingCount = 3, int page = 1, int pageSize = 20)
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

        var items = owned.Concat(missing).ToList();
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
