using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.DataMocks.Requests;

public static class UpdateUserProfileRequestMock
{
    public static UpdateUserProfileRequest Valid() => new() { Name = "Updated Name", Email = null };

    public static UpdateUserProfileRequest WithNewEmail(string email) => new() { Email = email, Name = null };

    public static UpdateUserProfileRequest WithBothFields(string name, string email) => new() { Name = name, Email = email };

    public static UpdateUserProfileRequest Empty() => new() { Name = null, Email = null };
}
