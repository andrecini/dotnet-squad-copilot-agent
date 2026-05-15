using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;

public static class CollectionItemModelMock
{
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
}
