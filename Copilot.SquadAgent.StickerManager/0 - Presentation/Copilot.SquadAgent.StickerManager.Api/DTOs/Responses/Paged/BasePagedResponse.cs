namespace Copilot.SquadAgent.StickerManager.Api.DTOs.Responses.Paged
{
    public class BasePagedResponse
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
