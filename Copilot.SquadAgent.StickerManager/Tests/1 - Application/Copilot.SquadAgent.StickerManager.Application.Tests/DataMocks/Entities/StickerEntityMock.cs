using Copilot.SquadAgent.StickerManager.Domain.Enums;
using StickerEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Sticker;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Entities;

public static class StickerEntityMock
{
    public static StickerEntity Valid() => new()
    {
        Code = "BRA001",
        PlayerName = "Neymar Jr",
        Rarity = StickerRarity.Base,
        TeamId = Guid.NewGuid()
    };
}
