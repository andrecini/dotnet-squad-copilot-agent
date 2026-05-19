using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class ResetPasswordRequestMock
{
    public static ResetPasswordRequest Valid() => new()
    {
        Token = Guid.NewGuid().ToString("N"),
        NewPassword = "NovaSenha@123"
    };

    public static ResetPasswordRequest WithEmptyToken() => new()
    {
        Token = string.Empty,
        NewPassword = "NovaSenha@123"
    };

    public static ResetPasswordRequest WithWeakPassword() => new()
    {
        Token = Guid.NewGuid().ToString("N"),
        NewPassword = "fraca"
    };

    public static ResetPasswordRequest WithNoUpperCase() => new()
    {
        Token = Guid.NewGuid().ToString("N"),
        NewPassword = "semmaius1234"
    };

    public static ResetPasswordRequest WithNoNumber() => new()
    {
        Token = Guid.NewGuid().ToString("N"),
        NewPassword = "SemNumeroAqui"
    };
}
