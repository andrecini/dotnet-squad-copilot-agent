using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Models.Collection;
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
            .FirstOrDefaultAsync(x => x.UserId == userId && x.StickerId == stickerId && x.DeletedAt == null, cancellationToken);

        return Result<UserCollectionEntity?>.Success(userCollection);
    }

    public async Task<Result<UserCollectionEntity?>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var userCollection = await dbContext.UserCollections
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

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

    public async Task<Result> SoftDeleteAsync(UserCollectionEntity userCollection, CancellationToken cancellationToken)
    {
        userCollection.DeletedAt = DateTime.UtcNow;
        dbContext.UserCollections.Update(userCollection);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<CollectionItemModel>>> ListByUserAsync(ListCollectionModel filter, CancellationToken cancellationToken)
    {
        var query = dbContext.UserCollections
            .AsNoTracking()
            .Where(uc => uc.UserId == filter.UserId && uc.DeletedAt == null)
            .Include(uc => uc.Sticker)
                .ThenInclude(s => s.Team)
            .AsQueryable();

        if (filter.TeamId.HasValue)
            query = query.Where(uc => uc.Sticker.TeamId == filter.TeamId.Value);

        if (filter.Rarity.HasValue)
            query = query.Where(uc => uc.Sticker.Rarity == filter.Rarity.Value);

        query = filter.Sort switch
        {
            "player_name" => query.OrderBy(uc => uc.Sticker.PlayerName),
            "acquired_at"  => query.OrderBy(uc => uc.CreatedAt),
            _              => query.OrderBy(uc => uc.Sticker.Code)
        };

        var items = await query
            .Skip((filter.Page - 1) * filter.Limit)
            .Take(filter.Limit)
            .Select(uc => new CollectionItemModel
            {
                StickerId        = uc.StickerId,
                PlayerName       = uc.Sticker.PlayerName,
                TeamName         = uc.Sticker.Team.Name,
                TeamCode         = uc.Sticker.Team.Code,
                Rarity           = uc.Sticker.Rarity,
                QuantityOwned    = uc.QuantityOwned,
                QuantityDuplicate = uc.QuantityDuplicate,
                AcquiredAt       = uc.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<CollectionItemModel>>.Success(items);
    }
}
