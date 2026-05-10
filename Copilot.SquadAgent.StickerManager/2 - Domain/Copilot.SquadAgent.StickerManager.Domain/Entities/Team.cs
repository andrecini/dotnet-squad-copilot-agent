namespace Copilot.SquadAgent.StickerManager.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string FlagUrl { get; set; } = string.Empty;

    public ICollection<Sticker> Stickers { get; set; } = [];
}
