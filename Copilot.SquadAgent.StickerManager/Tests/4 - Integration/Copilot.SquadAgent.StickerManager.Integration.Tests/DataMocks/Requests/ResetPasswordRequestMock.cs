using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;

public static class ResetPasswordRequestMock
{
    public static ResetPasswordRequest Valid(string token) => new()
    {
        Token = token,
        NewPassword = "NewPass@1234"
    };

    public static ResetPasswordRequest WithInvalidToken() => new()
    {
        Token = "invalid-token",
        NewPassword = "NewPass@1234"
    };

    public static ResetPasswordRequest WithWeakNewPassword(string token) => new()
    {
        Token = token,
        NewPassword = "123"
    };
}
