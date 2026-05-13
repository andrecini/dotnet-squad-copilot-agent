using PasswordResetTokenEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.PasswordResetToken;

namespace Copilot.SquadAgent.StickerManager.Application.Tests.DataMocks.Entities;

public static class PasswordResetTokenEntityMock
{
    public static PasswordResetTokenEntity Valid() => new()
    {
        UserId = Guid.NewGuid(),
        Token = Guid.NewGuid().ToString("N"),
        ExpiresAt = DateTime.UtcNow.AddHours(24),
        IsUsed = false,
        CreatedAt = DateTime.UtcNow
    };

    public static PasswordResetTokenEntity Expired() => new()
    {
        UserId = Guid.NewGuid(),
        Token = Guid.NewGuid().ToString("N"),
        ExpiresAt = DateTime.UtcNow.AddHours(-1),
        IsUsed = false,
        CreatedAt = DateTime.UtcNow.AddHours(-25)
    };

    public static PasswordResetTokenEntity AlreadyUsed() => new()
    {
        UserId = Guid.NewGuid(),
        Token = Guid.NewGuid().ToString("N"),
        ExpiresAt = DateTime.UtcNow.AddHours(23),
        IsUsed = true,
        CreatedAt = DateTime.UtcNow.AddHours(-1)
    };
}
