using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class TeamProgressModelMock
{
    public static TeamProgressModel Valid() => new()
    {
        TeamId               = Guid.NewGuid(),
        TeamName             = "Brasil",
        TotalStickers        = 20,
        OwnedCount           = 15,
        CompletionPercentage = 75.0
    };

    public static TeamProgressModel WithNoStickers() => new()
    {
        TeamId               = Guid.NewGuid(),
        TeamName             = "Argentina",
        TotalStickers        = 20,
        OwnedCount           = 0,
        CompletionPercentage = 0.0
    };

    public static TeamProgressModel Complete() => new()
    {
        TeamId               = Guid.NewGuid(),
        TeamName             = "França",
        TotalStickers        = 20,
        OwnedCount           = 20,
        CompletionPercentage = 100.0
    };
}
