using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using System.Diagnostics.CodeAnalysis;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;

[ExcludeFromCodeCoverage]
public static class RegisterUserRequestMock
{
    public static RegisterUserRequest Valid(string? email = null) => new()
    {
        Email = email ?? $"user_{Guid.NewGuid():N}@test.com",
        Name = "Test User",
        Password = "Test@1234"
    };

    public static RegisterUserRequest WithDuplicateEmail(string email) => new()
    {
        Email = email,
        Name = "Another User",
        Password = "Other@5678"
    };

    public static RegisterUserRequest WithInvalidEmail() => new()
    {
        Email = "email-invalido",
        Name = "Test User",
        Password = "Test@1234"
    };

    public static RegisterUserRequest WithWeakPassword() => new()
    {
        Email = $"user_{Guid.NewGuid():N}@test.com",
        Name = "Test User",
        Password = "123"
    };

    public static RegisterUserRequest WithEmptyName() => new()
    {
        Email = $"user_{Guid.NewGuid():N}@test.com",
        Name = string.Empty,
        Password = "Test@1234"
    };
}
