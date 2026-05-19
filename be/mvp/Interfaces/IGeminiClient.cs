using mvp.Models;

namespace mvp.Interfaces;

public interface IGeminiClient
{
    Task<AiSuggestions> GenerateSuggestionsAsync(string reviewText, int rating, CancellationToken cancellationToken);
}
