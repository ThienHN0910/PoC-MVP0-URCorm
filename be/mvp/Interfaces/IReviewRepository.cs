using mvp.Models;

namespace mvp.Interfaces;

public interface IReviewRepository
{
    Task<IReadOnlyList<ReviewDocument>> GetReviewsAsync(string? placeId, CancellationToken cancellationToken);

    Task<ReviewDocument?> GetReviewByIdAsync(string reviewId, CancellationToken cancellationToken);

    Task<bool> UpdateReplyAsync(string reviewId, string selectedReply, CancellationToken cancellationToken);

    Task<bool> UpdateAiSuggestionsAsync(string reviewId, AiSuggestions aiSuggestions, CancellationToken cancellationToken);

    Task SeedAsync(CancellationToken cancellationToken);
}
