using Backend.Application.Helpers;
using Backend.Application.Models;
using Backend.Core.Exceptions;
using Backend.Core.Interfaces.Repositories;
using Backend.Core.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Application.Services
{
  public class PostService : IPostService
  {
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
      _postRepository = postRepository;
    }

    public async Task<Post> CreatePost(PostInputModel model, User author)
    {
      string slug = SlugHelper.Slugify(model.Title);
      if (await _postRepository.GetBySlug(slug) != null)
        throw new PostAlreadyExistsException("Post with this title already exists");

      Post post = new()
      {
        Title = model.Title,
        Summary = model.Summary,
        Content = model.Content,
        Slug = slug,
        CreatedAt = DateTime.UtcNow,
        Author = author
      };

      return await _postRepository.Create(post);
    }

    public async Task<Post> UpdatePost(string identifier, UpdatePostModel model, User author)
    {
      Post post;
      if (ObjectId.TryParse(identifier, out ObjectId objectId))
      {
        post = await _postRepository.GetById(objectId.ToString());
        if (post == null)
          throw new PostNotFoundException("Post with this id does not exist");
      }
      else
      {
        post = await _postRepository.GetBySlug(identifier);
        if (post == null)
          throw new PostNotFoundException("Post with this id does not exist");
      }

      if (post.Author.Id != author.Id)
        throw new AuthorizationException("You are not the author of this post");

      post.Title = model.Title ?? post.Title;
      post.Summary = model.Summary ?? post.Summary;
      post.Content = model.Content ?? post.Content;
      post.Slug = SlugHelper.Slugify(model.Title ?? post.Title);
      post.UpdatedAt = DateTime.UtcNow;

      return await _postRepository.Update(post);
    }

    public async Task<List<PostViewModel>> GetPosts(int pageNumber, int pageSize)
    {
      List<Post> posts = await _postRepository.GetAll();
      int totalPosts = posts.Count;
      int totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
      List<Post> paginatedPosts = posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

      return paginatedPosts.Select(post => new PostViewModel
      {
        Id = post.Id,
        Title = post.Title,
        Summary = post.Summary,
        Content = post.Content,
        Slug = post.Slug,
        CreatedAt = post.CreatedAt
      }).ToList();
    }

    public async Task<PostViewModel?> GetPostByIdentifier(string identifier)
    {
      Post post;
      if (ObjectId.TryParse(identifier, out ObjectId objectId))
      {
        post = await _postRepository.GetById(objectId.ToString());
      }
      else
      {
        post = await _postRepository.GetBySlug(identifier);
      }

      if (post == null)
        return null;

      return new PostViewModel
      {
        Id = post.Id,
        Title = post.Title,
        Summary = post.Summary,
        Content = post.Content,
        Slug = post.Slug,
        CreatedAt = post.CreatedAt
      };
    }
  }
}
