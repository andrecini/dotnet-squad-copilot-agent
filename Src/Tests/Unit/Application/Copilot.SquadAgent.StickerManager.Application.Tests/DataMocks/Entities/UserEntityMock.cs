using UserEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Entities;

public static class UserEntityMock
{
    public static UserEntity Valid() => new()
    {
        Email = "joao.silva@email.com",
        Name = "João Silva",
        PasswordHash = "hashed_password",
        CreatedAt = DateTime.UtcNow
    };
}
