using mvp.Interfaces;
using mvp.Models;

namespace mvp.Services;

public sealed class ReviewService(IReviewRepository reviewRepository, IGeminiClient geminiClient) : IReviewService
{
    public async Task<IReadOnlyList<GetDataResponseItem>> GetDataAsync(string? placeId, CancellationToken cancellationToken)
    {
        var reviews = await reviewRepository.GetReviewsAsync(placeId, cancellationToken);
        return reviews
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new GetDataResponseItem
            {
                ReviewId = r.Id,
                PlaceId = r.PlaceId,
                AuthorName = r.AuthorName,
                Rating = r.Rating,
                ReviewText = r.ReviewText,
                AiSuggestions = r.AiSuggestions,
                SelectedReply = r.SelectedReply,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToList();
    }

    public Task<bool> ResolveReviewAsync(string reviewId, string selectedReply, CancellationToken cancellationToken)
        => reviewRepository.UpdateReplyAsync(reviewId, selectedReply, cancellationToken);

    public async Task<AiSuggestions> GenerateAndSaveSuggestionsAsync(string reviewId, string reviewText, int rating, CancellationToken cancellationToken)
    {
        var suggestions = await geminiClient.GenerateSuggestionsAsync(reviewText, rating, cancellationToken);
        var updated = await reviewRepository.UpdateAiSuggestionsAsync(reviewId, suggestions, cancellationToken);

        if (!updated)
        {
            throw new KeyNotFoundException($"Review '{reviewId}' was not found.");
        }

        return suggestions;
    }
}
