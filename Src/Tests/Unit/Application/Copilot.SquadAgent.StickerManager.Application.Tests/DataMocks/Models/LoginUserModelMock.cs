using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class LoginUserModelMock
{
    public static LoginUserModel Valid() => new()
    {
        Email = "joao.silva@email.com",
        Password = "Senha123"
    };

    public static LoginUserModel WithWrongPassword() => new()
    {
        Email = "joao.silva@email.com",
        Password = "SenhaErrada99"
    };
}
