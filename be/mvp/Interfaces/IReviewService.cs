using mvp.Models;

namespace mvp.Interfaces;

public interface IReviewService
{
    Task<IReadOnlyList<GetDataResponseItem>> GetDataAsync(CancellationToken cancellationToken);

    Task<bool> ResolveReviewAsync(string reviewId, string selectedReply, CancellationToken cancellationToken);

    Task<AiSuggestions> GenerateAndSaveSuggestionsAsync(string reviewId, string reviewText, int rating, CancellationToken cancellationToken);
}
