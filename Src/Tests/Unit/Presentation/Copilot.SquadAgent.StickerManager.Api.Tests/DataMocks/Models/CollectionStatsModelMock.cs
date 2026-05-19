using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Models;

public static class CollectionStatsModelMock
{
    public static CollectionStatsModel Valid() => new()
    {
        TotalOwned           = 100,
        TotalMissing         = 550,
        TotalStickers        = 650,
        CompletionPercentage = 15.4,
        Duplicates           = 8,
        ByTeam               =
        [
            new TeamStatsModel
            {
                TeamId               = Guid.NewGuid(),
                TeamName             = "Brasil",
                TeamCode             = "BRA",
                Owned                = 15,
                Total                = 20,
                CompletionPercentage = 75.0
            },
            new TeamStatsModel
            {
                TeamId               = Guid.NewGuid(),
                TeamName             = "Argentina",
                TeamCode             = "ARG",
                Owned                = 10,
                Total                = 20,
                CompletionPercentage = 50.0
            }
        ]
    };

    public static CollectionStatsModel Empty() => new()
    {
        TotalOwned           = 0,
        TotalMissing         = 650,
        TotalStickers        = 650,
        CompletionPercentage = 0,
        Duplicates           = 0,
        ByTeam               = []
    };
}
