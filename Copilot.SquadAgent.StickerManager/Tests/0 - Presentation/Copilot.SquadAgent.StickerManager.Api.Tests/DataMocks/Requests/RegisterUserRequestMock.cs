using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Api.Tests.DataMocks.Requests;

public static class RegisterUserRequestMock
{
    public static RegisterUserRequest Valid() => new()
    {
        Email = "joao.silva@email.com",
        Name = "João Silva",
        Password = "Senha123"
    };

    public static RegisterUserRequest WithEmptyEmail() => new()
    {
        Email = string.Empty,
        Name = "João Silva",
        Password = "Senha123"
    };

    public static RegisterUserRequest WithInvalidEmail() => new()
    {
        Email = "email-invalido",
        Name = "João Silva",
        Password = "Senha123"
    };

    public static RegisterUserRequest WithWeakPassword() => new()
    {
        Email = "joao.silva@email.com",
        Name = "João Silva",
        Password = "fraca"
    };

    public static RegisterUserRequest WithPasswordWithoutUppercase() => new()
    {
        Email = "joao.silva@email.com",
        Name = "João Silva",
        Password = "senha1234"
    };

    public static RegisterUserRequest WithPasswordWithoutNumber() => new()
    {
        Email = "joao.silva@email.com",
        Name = "João Silva",
        Password = "SenhaSemNumero"
    };

    public static RegisterUserRequest WithEmptyName() => new()
    {
        Email = "joao.silva@email.com",
        Name = string.Empty,
        Password = "Senha123"
    };
}
