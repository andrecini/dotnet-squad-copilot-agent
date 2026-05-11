using UserEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.User;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;

public static class UserEntityMock
{
    public static UserEntity Valid() => new()
    {
        Email = "joao.silva@email.com",
        Name = "João Silva",
        PasswordHash = "hashed_password"
    };

    public static UserEntity WithEmail(string email) => new()
    {
        Email = email,
        Name = "João Silva",
        PasswordHash = "hashed_password"
    };
}
