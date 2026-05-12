using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class ForgotPasswordModelMock
{
    public static ForgotPasswordModel Valid() => new()
    {
        Email = "joao.silva@email.com"
    };

    public static ForgotPasswordModel WithEmail(string email) => new()
    {
        Email = email
    };
}
