using PasswordResetTokenEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.PasswordResetToken;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Tests.DataMocks.Entities;

public static class PasswordResetTokenEntityMock
{
    public static PasswordResetTokenEntity Valid(Guid userId) => new()
    {
        UserId = userId,
        Token = Guid.NewGuid().ToString("N"),
        ExpiresAt = DateTime.UtcNow.AddHours(24),
        IsUsed = false,
        CreatedAt = DateTime.UtcNow
    };

    public static PasswordResetTokenEntity Expired(Guid userId) => new()
    {
        UserId = userId,
        Token = Guid.NewGuid().ToString("N"),
        ExpiresAt = DateTime.UtcNow.AddHours(-1),
        IsUsed = false,
        CreatedAt = DateTime.UtcNow.AddHours(-25)
    };

    public static PasswordResetTokenEntity AlreadyUsed(Guid userId) => new()
    {
        UserId = userId,
        Token = Guid.NewGuid().ToString("N"),
        ExpiresAt = DateTime.UtcNow.AddHours(23),
        IsUsed = true,
        CreatedAt = DateTime.UtcNow.AddHours(-1)
    };
}
