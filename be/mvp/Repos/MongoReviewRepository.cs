using Microsoft.Extensions.Options;
using MongoDB.Driver;
using mvp.Interfaces;
using mvp.Models;
using mvp.Options;

namespace mvp.Repos;

public sealed class MongoReviewRepository : IReviewRepository
{
    private readonly IMongoCollection<PlaceDocument> _places;
    private readonly IMongoCollection<ReviewDocument> _reviews;

    public MongoReviewRepository(IOptions<MongoDbOptions> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _places = database.GetCollection<PlaceDocument>(settings.PlacesCollectionName);
        _reviews = database.GetCollection<ReviewDocument>(settings.ReviewsCollectionName);
    }

    public async Task<IReadOnlyList<ReviewDocument>> GetReviewsAsync(CancellationToken cancellationToken)
        => await _reviews.Find(Builders<ReviewDocument>.Filter.Empty).ToListAsync(cancellationToken);

    public async Task<ReviewDocument?> GetReviewByIdAsync(string reviewId, CancellationToken cancellationToken)
        => await _reviews.Find(x => x.Id == reviewId).FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> UpdateReplyAsync(string reviewId, string selectedReply, CancellationToken cancellationToken)
    {
        var update = Builders<ReviewDocument>.Update
            .Set(x => x.SelectedReply, selectedReply)
            .Set(x => x.Status, ReviewStatus.Resolved);

        var result = await _reviews.UpdateOneAsync(x => x.Id == reviewId, update, cancellationToken: cancellationToken);
        return result.ModifiedCount == 1;
    }

    public async Task<bool> UpdateAiSuggestionsAsync(string reviewId, AiSuggestions aiSuggestions, CancellationToken cancellationToken)
    {
        var update = Builders<ReviewDocument>.Update.Set(x => x.AiSuggestions, aiSuggestions);
        var result = await _reviews.UpdateOneAsync(x => x.Id == reviewId, update, cancellationToken: cancellationToken);
        return result.ModifiedCount == 1;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var placeExists = await _places.Find(x => x.PlaceId == "vinpearl-luxury-landmark-81").AnyAsync(cancellationToken);

        if (!placeExists)
        {
            await _places.InsertOneAsync(new PlaceDocument
            {
                PlaceId = "vinpearl-luxury-landmark-81",
                DisplayName = "Vinpearl Luxury Landmark 81",
                CreatedAt = DateTime.UtcNow
            }, cancellationToken: cancellationToken);
        }

        var reviewCount = await _reviews.CountDocumentsAsync(x => x.PlaceId == "vinpearl-luxury-landmark-81", cancellationToken: cancellationToken);

        if (reviewCount > 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var sampleReviews = new[]
        {
            new ReviewDocument
            {
                PlaceId = "vinpearl-luxury-landmark-81",
                AuthorName = "Ngọc Anh",
                Rating = 5,
                ReviewText = "Dịch vụ tuyệt vời, phòng đẹp và nhân viên rất nhiệt tình.",
                Status = ReviewStatus.Pending,
                CreatedAt = now.AddMinutes(-50)
            },
            new ReviewDocument
            {
                PlaceId = "vinpearl-luxury-landmark-81",
                AuthorName = "Minh Tuấn",
                Rating = 1,
                ReviewText = "Check-in quá lâu, phòng chưa được dọn kỹ.",
                Status = ReviewStatus.Pending,
                CreatedAt = now.AddMinutes(-40)
            },
            new ReviewDocument
            {
                PlaceId = "vinpearl-luxury-landmark-81",
                AuthorName = "Hải Yến",
                Rating = 2,
                ReviewText = "Bữa sáng chưa đa dạng, món ăn nguội nhanh.",
                Status = ReviewStatus.Pending,
                CreatedAt = now.AddMinutes(-30)
            },
            new ReviewDocument
            {
                PlaceId = "vinpearl-luxury-landmark-81",
                AuthorName = "Quốc Bảo",
                Rating = 5,
                ReviewText = "View thành phố cực đẹp, sẽ quay lại lần sau.",
                Status = ReviewStatus.Pending,
                CreatedAt = now.AddMinutes(-20)
            },
            new ReviewDocument
            {
                PlaceId = "vinpearl-luxury-landmark-81",
                AuthorName = "Thu Phương",
                Rating = 4,
                ReviewText = "Khá hài lòng, chỉ có wifi đôi lúc chập chờn.",
                Status = ReviewStatus.Pending,
                CreatedAt = now.AddMinutes(-10)
            }
        };

        await _reviews.InsertManyAsync(sampleReviews, cancellationToken: cancellationToken);
    }
}
