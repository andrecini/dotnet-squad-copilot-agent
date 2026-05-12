using Copilot.SquadAgent.StickerManager.Domain.Models.User;

namespace Copilot.SquadAgent.StickerManager.Domain.Interfaces.Security;

public interface IJwtTokenGenerator
{
    TokenModel Generate(UserModel user);
}
