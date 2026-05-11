namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests;

public class RegisterUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
