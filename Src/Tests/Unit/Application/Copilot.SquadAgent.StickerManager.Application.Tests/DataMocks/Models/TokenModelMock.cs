using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class TokenModelMock
{
    public static TokenModel Valid() => new()
    {
        AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.fake.token",
        TokenType = "Bearer",
        ExpiresIn = 3600
    };
}
