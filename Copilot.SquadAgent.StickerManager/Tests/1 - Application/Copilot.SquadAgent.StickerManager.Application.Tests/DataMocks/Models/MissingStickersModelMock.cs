using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class MissingStickersModelMock
{
    public static MissingStickersModel Valid(Guid? userId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid()
    };

    public static MissingStickersModel WithSort(Guid? userId = null, string sort = "rarity") => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Sort = sort
    };

    public static MissingStickersModel WithTeamSort(Guid? userId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Sort = "team"
    };

    public static MissingStickersModel WithNumberSort(Guid? userId = null) => new()
    {
        UserId = userId ?? Guid.NewGuid(),
        Sort = "number"
    };
}
