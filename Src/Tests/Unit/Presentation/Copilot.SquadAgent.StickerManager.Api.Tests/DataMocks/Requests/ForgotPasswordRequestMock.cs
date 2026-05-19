using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class ForgotPasswordRequestMock
{
    public static ForgotPasswordRequest Valid() => new()
    {
        Email = "joao.silva@email.com"
    };

    public static ForgotPasswordRequest WithEmptyEmail() => new()
    {
        Email = string.Empty
    };

    public static ForgotPasswordRequest WithInvalidEmail() => new()
    {
        Email = "nao-e-um-email"
    };
}
