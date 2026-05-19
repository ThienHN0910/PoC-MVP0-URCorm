using mvp.Models;
namespace mvp.Interfaces;

public sealed class GetDataResponseItem
{
    public string ReviewId { get; set; } = string.Empty;
    public string PlaceId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string ReviewText { get; set; } = string.Empty;
    public AiSuggestions? AiSuggestions { get; set; }
    public string? SelectedReply { get; set; }
    public string Status { get; set; } = ReviewStatus.Pending;
    public DateTime CreatedAt { get; set; }
}
