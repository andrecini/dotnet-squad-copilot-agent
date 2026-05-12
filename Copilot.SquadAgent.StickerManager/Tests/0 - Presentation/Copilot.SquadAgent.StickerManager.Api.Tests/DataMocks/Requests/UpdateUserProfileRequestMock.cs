using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class UpdateUserProfileRequestMock
{
    public static UpdateUserProfileRequest Valid() => new()
    {
        Email = "novo.email@email.com",
        Name = "João Atualizado"
    };

    public static UpdateUserProfileRequest WithOnlyName() => new()
    {
        Email = null,
        Name = "Novo Nome"
    };

    public static UpdateUserProfileRequest WithOnlyEmail() => new()
    {
        Email = "novo.email@email.com",
        Name = null
    };

    public static UpdateUserProfileRequest WithInvalidEmail() => new()
    {
        Email = "email-invalido",
        Name = null
    };

    public static UpdateUserProfileRequest WithShortName() => new()
    {
        Email = null,
        Name = "A"
    };

    public static UpdateUserProfileRequest WithBothNull() => new()
    {
        Email = null,
        Name = null
    };
}
