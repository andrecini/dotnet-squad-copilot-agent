using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;

[ExcludeFromCodeCoverage]
public static class LoginUserRequestMock
{
    public static LoginUserRequest Valid(string email = "login@test.com") => new()
    {
        Email = email,
        Password = "Test@1234"
    };

    public static LoginUserRequest WithWrongPassword(string email = "login@test.com") => new()
    {
        Email = email,
        Password = "Wrong@9999"
    };

    public static LoginUserRequest WithInvalidEmail() => new()
    {
        Email = "email-invalido",
        Password = "Test@1234"
    };
}
