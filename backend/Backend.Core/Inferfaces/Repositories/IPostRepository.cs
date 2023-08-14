using Backend.Core.Models;

namespace Backend.Core.Inferfaces.Repositories;

public interface IPostRepository
{
    Task<Post> Create(Post post);
    Task<Post> Update(Post post);
    Task<Post> GetById(string id);
    Task<Post> GetBySlug(string slug);
    Task<List<Post>> GetAll();
}
