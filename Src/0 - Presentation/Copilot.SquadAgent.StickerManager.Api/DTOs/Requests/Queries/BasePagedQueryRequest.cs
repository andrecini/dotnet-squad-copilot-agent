namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Requests.Queries
{
    public class BasePagedQueryRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
