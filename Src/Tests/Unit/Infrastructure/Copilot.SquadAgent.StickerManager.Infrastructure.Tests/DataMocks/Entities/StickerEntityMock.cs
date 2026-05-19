using Copilot.SquadAgent.StickerManager.Domain.Enums;
using StickerEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Sticker;
using TeamEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Team;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;

public static class StickerEntityMock
{
    public static StickerEntity Valid() => new()
    {
        Code = "BRA001",
        PlayerName = "Neymar Jr",
        Rarity = StickerRarity.Base,
        TeamId = Guid.NewGuid()
    };

    public static StickerEntity ValidWithTeam(TeamEntity team) => new()
    {
        Code = "BRA001",
        PlayerName = "Neymar Jr",
        Rarity = StickerRarity.Base,
        TeamId = team.Id,
        Team = team
    };

    public static StickerEntity ValidWithTeamAndRarity(TeamEntity team, StickerRarity rarity) => new()
    {
        Code = "BRA002",
        PlayerName = "Vinicius Jr",
        Rarity = rarity,
        TeamId = team.Id,
        Team = team
    };
}
