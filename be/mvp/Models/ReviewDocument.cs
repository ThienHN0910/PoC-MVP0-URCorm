using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mvp.Models;

public sealed class ReviewDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("placeId")]
    public string PlaceId { get; set; } = string.Empty;

    [BsonElement("authorName")]
    public string AuthorName { get; set; } = string.Empty;

    [BsonElement("rating")]
    public int Rating { get; set; }

    [BsonElement("reviewText")]
    public string ReviewText { get; set; } = string.Empty;

    [BsonElement("aiSuggestions")]
    [BsonIgnoreIfNull]
    public AiSuggestions? AiSuggestions { get; set; }

    [BsonElement("selectedReply")]
    [BsonIgnoreIfNull]
    public string? SelectedReply { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = ReviewStatus.Pending;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
}
