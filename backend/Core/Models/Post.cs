using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Core.Models;

public class Post
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }

  public string Title { get; set; }
  public string Summary { get; set; }
  public string Content { get; set; }
  public string Cover { get; set; }

  [BsonIgnore]
  public User Author { get; set; }

  [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
  public DateTime CreatedAt { get; } = DateTime.Now;

  [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
}