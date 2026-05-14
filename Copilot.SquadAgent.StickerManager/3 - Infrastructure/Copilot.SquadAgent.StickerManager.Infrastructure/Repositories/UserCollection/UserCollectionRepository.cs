using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UserCollectionEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.UserCollection;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.UserCollection;

public class UserCollectionRepository(AppDbContext dbContext) : IUserCollectionRepository
{
    public async Task<Result<UserCollectionEntity?>> GetByUserAndStickerAsync(Guid userId, Guid stickerId, CancellationToken cancellationToken)
    {
        var userCollection = await dbContext.UserCollections
            .FirstOrDefaultAsync(x => x.UserId == userId && x.StickerId == stickerId, cancellationToken);

        return Result<UserCollectionEntity?>.Success(userCollection);
    }

    public async Task<Result<UserCollectionEntity>> CreateAsync(UserCollectionEntity userCollection, CancellationToken cancellationToken)
    {
        await dbContext.UserCollections.AddAsync(userCollection, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<UserCollectionEntity>.Success(userCollection);
    }

    public async Task<Result<UserCollectionEntity>> UpdateAsync(UserCollectionEntity userCollection, CancellationToken cancellationToken)
    {
        dbContext.UserCollections.Update(userCollection);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<UserCollectionEntity>.Success(userCollection);
    }
}
