using Copilot.SquadAgent.StickerManager.Domain.Enums;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

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
}
