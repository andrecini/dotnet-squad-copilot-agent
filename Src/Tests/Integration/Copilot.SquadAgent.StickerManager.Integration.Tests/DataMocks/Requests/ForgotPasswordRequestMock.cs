using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;

[ExcludeFromCodeCoverage]
public static class ForgotPasswordRequestMock
{
    public static ForgotPasswordRequest Valid(string email = "forgot@test.com") => new()
    {
        Email = email
    };

    public static ForgotPasswordRequest WithUnknownEmail() => new()
    {
        Email = $"unknown_{Guid.NewGuid():N}@test.com"
    };

    public static ForgotPasswordRequest WithInvalidEmail() => new()
    {
        Email = "email-invalido"
    };
}
