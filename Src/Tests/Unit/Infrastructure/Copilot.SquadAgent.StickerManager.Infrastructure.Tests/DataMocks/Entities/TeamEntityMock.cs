using TeamEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Team;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;

public static class TeamEntityMock
{
    public static TeamEntity Valid() => new()
    {
        Name = "Brasil",
        Code = "BRA",
        FlagUrl = "https://flags.example.com/bra.png"
    };
}
