using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StickerEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.Sticker;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.Sticker;

public class StickerRepository(AppDbContext dbContext) : IStickerRepository
{
    public async Task<Result<StickerEntity>> GetByIdAsync(Guid stickerId, CancellationToken cancellationToken)
    {
        var sticker = await dbContext.Stickers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == stickerId, cancellationToken);

        if (sticker is null)
            return Result<StickerEntity>.Failure(ResultCode.NotFound, "Figurinha não encontrada.", statusCode: 404);

        return Result<StickerEntity>.Success(sticker);
    }
}
