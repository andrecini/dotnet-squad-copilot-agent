using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class ResetPasswordModelMock
{
    public static ResetPasswordModel Valid() => new()
    {
        Token = Guid.NewGuid().ToString("N"),
        NewPassword = "NovaSenha@123"
    };

    public static ResetPasswordModel WithToken(string token) => new()
    {
        Token = token,
        NewPassword = "NovaSenha@123"
    };
}
