using System.Text.Json;
using Google.GenAI;
using Microsoft.Extensions.Options;
using mvp.Interfaces;
using mvp.Models;
using mvp.Options;

namespace mvp.Services;

public sealed class GeminiClient(HttpClient httpClient, IOptions<GeminiOptions> options) : IGeminiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly GeminiOptions _options = options.Value;

    public async Task<AiSuggestions> GenerateSuggestionsAsync(string reviewText, int rating, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("Gemini:ApiKey is required.");
        }

        _ = httpClient;

        var prompt = $"{_options.Prompt}\n\nRating: {rating}/5\nReview: {reviewText}\nTrả về JSON với 3 key: standard, friendly, apology.";

        var client = new Client(apiKey: _options.ApiKey);

        var response = await client.Models.GenerateContentAsync(
            model: _options.Model,
            contents: prompt
        );

        var text = response?.Candidates?[0]?.Content?.Parts?[0]?.Text;

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("Gemini response content is empty.");
        }

        var parsed = JsonSerializer.Deserialize<AiSuggestions>(text, JsonOptions);

        return parsed is null
            ? throw new InvalidOperationException("Gemini JSON parsing failed.")
            : parsed;
    }
}
