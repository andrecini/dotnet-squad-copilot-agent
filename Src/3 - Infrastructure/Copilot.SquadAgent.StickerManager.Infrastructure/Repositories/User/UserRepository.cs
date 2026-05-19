using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Repositories;
using Copilot.SquadAgent.StickerManager.Domain.Result;
using Copilot.SquadAgent.StickerManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UserEntity = Copilot.SquadAgent.StickerManager.Domain.Entities.User;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Repositories.User;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<Result<UserEntity>> CreateAsync(UserEntity user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<UserEntity>.Success(user);
    }

    public async Task<Result<UserEntity>> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email && x.DeletedAt == null, cancellationToken);

        if (user is null)
            return Result<UserEntity>.Failure(ResultCode.NotFound, "Usuário não encontrado.", statusCode: 404);

        return Result<UserEntity>.Success(user);
    }

    public async Task<Result<UserEntity>> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
            return Result<UserEntity>.Failure(ResultCode.NotFound, "Usuário não encontrado.", statusCode: 404);

        return Result<UserEntity>.Success(user);
    }

    public async Task<Result<UserEntity>> UpdateAsync(UserEntity user, CancellationToken cancellationToken)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<UserEntity>.Success(user);
    }
}