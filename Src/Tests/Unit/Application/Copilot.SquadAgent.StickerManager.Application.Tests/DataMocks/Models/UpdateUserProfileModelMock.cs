using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Models;

public static class UpdateUserProfileModelMock
{
    public static UpdateUserProfileModel Valid() => new()
    {
        UserId = Guid.NewGuid(),
        Email = "novo.email@email.com",
        Name = "João Atualizado"
    };

    public static UpdateUserProfileModel WithOnlyName() => new()
    {
        UserId = Guid.NewGuid(),
        Email = null,
        Name = "Novo Nome"
    };

    public static UpdateUserProfileModel WithOnlyEmail() => new()
    {
        UserId = Guid.NewGuid(),
        Email = "novo.email@email.com",
        Name = null
    };

    public static UpdateUserProfileModel WithSameEmail(string currentEmail) => new()
    {
        UserId = Guid.NewGuid(),
        Email = currentEmail,
        Name = null
    };
}
