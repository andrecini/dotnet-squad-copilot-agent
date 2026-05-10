namespace Copilot.SquadAgent.StickerManager.Domain.Entities;

public class UserCollection : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid StickerId { get; set; }
    public int QuantityOwned { get; set; }
    public int QuantityDuplicate { get; set; }

    public User User { get; set; } = null!;
    public Sticker Sticker { get; set; } = null!;
}
