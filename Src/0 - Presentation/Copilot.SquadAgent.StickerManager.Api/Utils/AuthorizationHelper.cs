using Copilot.SquadAgent.StickerManager.Domain.Result;
using System.Security.Claims;

namespace Copilot.SquadAgent.StickerManager.Api.Utils
{
    public class AuthorizationHelper
    {
        public static Result<Guid> GetUserIdFromClaims(ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Result<Guid>.Failure(ResultCode.Unauthorized, "Usuário não autenticado");

            return Result<Guid>.Success(userId);
        }
    }
}
