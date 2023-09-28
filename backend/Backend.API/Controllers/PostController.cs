using Backend.API.Models;
using Backend.Application.Models;
using Backend.Application.Services;
using Backend.Application.Validators;
using Backend.Core.Models;
using Backend.Infrastructure.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.API.Controllers;

[Authorize(Roles = "Admin")]
public class PostController : BaseApiController
{
  private readonly IPostService _postService;
  private readonly UserManager<User> _userManager;
  private readonly ICachingService _cachingService;

  public PostController(IPostService postService, UserManager<User> userManager, ICachingService cachingService)
  {
    _postService = postService;
    _userManager = userManager;
    _cachingService = cachingService;
  }

  [HttpPost]
  [SwaggerOperation(Summary = "Create a new post.")]
  [ProducesResponseType(typeof(ApiResponse<PostViewModel>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<PostViewModel>>> CreatePost([FromBody] PostInputModel model)
  {
    FluentValidation.Results.ValidationResult validationResult = ValidateModel<PostInputModelValidator, PostInputModel>(model);

    if (!validationResult.IsValid)
      return BadRequest(new ApiResponse<PostViewModel>
      {
        Success = false,
        Errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList()
      });

    User currentUser = await _userManager.FindByIdAsync(model.AuthorId);

    Post post = await _postService.CreatePost(model, currentUser);

    return Ok(new ApiResponse<PostViewModel>
    {
      Success = true,
      Data = new PostViewModel
      {
        Id = post.Id,
        Title = post.Title,
        Summary = post.Summary,
        Slug = post.Slug,
        Content = post.Content
      }
    });
  }

  [HttpPut("{identifier}")]
  [SwaggerOperation(Summary = "Update a post by id or slug.")]
  [ProducesResponseType(typeof(ApiResponse<PostViewModel>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<ApiResponse<PostViewModel>>> UpdatePost(string identifier, [FromForm] UpdatePostModel model)
  {
    FluentValidation.Results.ValidationResult validationResult = ValidateModel<UpdatePostModelValidator, UpdatePostModel>(model);

    if (!validationResult.IsValid)
      return BadRequest(new ApiResponse<PostViewModel>
      {
        Success = false,
        Errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList()
      });

    User currentUser = await _userManager.FindByIdAsync(model.AuthorId);

    if (currentUser == null)
      return BadRequest(new ApiResponse<PostViewModel>
      {
        Success = false,
        Errors = new List<string> { "User not found" }
      });

    Post post = await _postService.UpdatePost(identifier, model, currentUser);

    return Ok(new ApiResponse<PostViewModel>
    {
      Success = true,
      Data = new PostViewModel
      {
        Id = post.Id,
        Title = post.Title,
        Summary = post.Summary,
        Slug = post.Slug,
        Content = post.Content
      }
    });
  }

  [AllowAnonymous]
  [HttpGet]
  [SwaggerOperation(Summary = "Get all posts.")]
  [ProducesResponseType(typeof(ApiResponse<List<PostViewModel>>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<IEnumerable<PostViewModel>>> GetPosts()
  {
    List<PostViewModel>? posts;
    var postsCache = await _cachingService.GetAsync("posts");

    if (!string.IsNullOrWhiteSpace(postsCache))
    {
      posts = JsonConvert.DeserializeObject<List<PostViewModel>>(postsCache);

      if (posts == null)
        return NotFound();

      return Ok(new ApiResponse<List<PostViewModel>>
      {
        Success = true,
        Data = posts
      });
    }

    posts = await _postService.GetPosts();

    if (posts == null)
      return NotFound();

    await _cachingService.SetAsync("posts", JsonConvert.SerializeObject(posts));

    return Ok(new ApiResponse<List<PostViewModel>>
    {
      Success = true,
      Data = posts
    });
  }

  [AllowAnonymous]
  [HttpGet("{identifier}")]
  [SwaggerOperation(Summary = "Get a post by ID or slug.")]
  [ProducesResponseType(typeof(ApiResponse<PostViewModel>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<ActionResult<PostViewModel>> GetPost(string identifier)
  {
    PostViewModel? post;
    var postCache = await _cachingService.GetAsync(identifier);

    if (!string.IsNullOrWhiteSpace(postCache))
    {
      post = JsonConvert.DeserializeObject<PostViewModel>(postCache);

      if (post == null)
        return NotFound();

      return Ok(new ApiResponse<PostViewModel>
      {
        Success = true,
        Data = post
      });
    }

    post = await _postService.GetPostByIdentifier(identifier);

    if (post == null)
      return NotFound();

    await _cachingService.SetAsync(identifier, JsonConvert.SerializeObject(post));

    return Ok(new ApiResponse<PostViewModel>
    {
      Success = true,
      Data = post
    });
  }
}
