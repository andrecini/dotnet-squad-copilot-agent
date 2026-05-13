using Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;
using Copilot.SquadAgent.StickerManager.Api.DTOs.Responses;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;

namespace Copilot.SquadAgent.StickerManager.Integration.Tests.Helpers;

[ExcludeFromCodeCoverage]
public static class AuthHelper
{
    public static async Task<string> GetTokenAsync(HttpClient client, string email = "auth@test.com", string password = "Test@1234")
    {
        var registerRequest = new RegisterUserRequest
        {
            Email = email,
            Name = "Test User",
            Password = password
        };

        await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = new LoginUserRequest
        {
            Email = email,
            Password = password
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var body = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
        return body!.AccessToken;
    }
}
