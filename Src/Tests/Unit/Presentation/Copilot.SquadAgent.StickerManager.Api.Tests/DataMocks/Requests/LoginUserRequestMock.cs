using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class LoginUserRequestMock
{
    public static LoginUserRequest Valid() => new()
    {
        Email = "joao.silva@email.com",
        Password = "Senha123"
    };

    public static LoginUserRequest WithEmptyEmail() => new()
    {
        Email = string.Empty,
        Password = "Senha123"
    };

    public static LoginUserRequest WithInvalidEmail() => new()
    {
        Email = "email-invalido",
        Password = "Senha123"
    };

    public static LoginUserRequest WithEmptyPassword() => new()
    {
        Email = "joao.silva@email.com",
        Password = string.Empty
    };
}
