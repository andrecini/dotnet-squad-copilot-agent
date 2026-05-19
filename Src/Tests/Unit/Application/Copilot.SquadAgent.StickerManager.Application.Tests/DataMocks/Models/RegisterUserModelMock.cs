using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class RegisterUserModelMock
{
    public static RegisterUserModel Valid() => new()
    {
        Email = "joao.silva@email.com",
        Name = "João Silva",
        Password = "Senha123"
    };
}
