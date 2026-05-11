using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class UserModelMock
{
    public static UserModel Valid() => new()
    {
        Id = Guid.NewGuid(),
        Email = "joao.silva@email.com",
        Name = "João Silva",
        CreatedAt = DateTime.UtcNow
    };
}
