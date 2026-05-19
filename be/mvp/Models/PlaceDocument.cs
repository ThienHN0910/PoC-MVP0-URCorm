using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mvp.Models;

public sealed class PlaceDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("placeId")]
    public string PlaceId { get; set; } = string.Empty;

    [BsonElement("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
}
