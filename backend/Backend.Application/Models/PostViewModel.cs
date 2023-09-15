using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Application.Models;

public class PostViewModel
{
  [SwaggerSchema(Description = "The post's id")]
  public string Id { get; set; }

  [SwaggerSchema(Description = "The post's slug")]
  public string Slug { get; set; }

  [SwaggerSchema(Description = "The post's date of creation")]
  public DateTime CreatedAt { get; set; }
}