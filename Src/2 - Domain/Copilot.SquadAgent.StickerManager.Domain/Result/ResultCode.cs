namespace Copilot.SquadAgent.StickerManager.Domain.Result;

public enum ResultCode
{
    Success,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    ValidationError,
    BusinessError,
    InternalError
}
