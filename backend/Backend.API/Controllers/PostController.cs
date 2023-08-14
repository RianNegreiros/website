using Backend.API.Models;
using Backend.Application.Models;
using Backend.Application.Services;
using Backend.Application.Validators;
using Backend.Core.Exceptions;
using Backend.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend.API.Controllers;

[Authorize(Roles = "Admin")]
public class PostController : BaseApiController
{
  private readonly IPostService _postService;
  private readonly UserManager<User> _userManager;

  public PostController(IPostService postService, UserManager<User> userManager)
  {
    _postService = postService;
    _userManager = userManager;
  }

  [HttpPost]
  public async Task<ActionResult<ApiResponse<PostViewModel>>> CreatePost([FromBody] PostInputModel model)
  {
    var validationResult = ValidateModel<PostInputModelValidator, PostInputModel>(model);

    if (!validationResult.IsValid)
      return BadRequest(new ApiResponse<PostViewModel>
      {
        Success = false,
        Errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList()
      });

    try
    {

      var currentUser = await _userManager.GetUserAsync(User);

      var post = await _postService.CreatePost(model, currentUser);

      var postViewModel = new PostViewModel
      {
        Id = post.Id,
        Title = post.Title,
        Summary = post.Summary,
        Slug = post.Slug,
        Content = post.Content
      };

      return Ok(postViewModel);
    }
    catch (PostNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
    catch (AuthorizationException ex)
    {
      return Unauthorized(ex.Message);
    }
    catch (ImageUploadException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (Exception)
    {
      return BadRequest("An error occurred while processing the request.");
    }
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ApiResponse<PostViewModel>>> UpdatePost(string id, [FromForm] PostInputModel model)
  {
    var validationResult = ValidateModel<PostInputModelValidator, PostInputModel>(model);

    if (!validationResult.IsValid)
      return BadRequest(new ApiResponse<PostViewModel>
      {
        Success = false,
        Errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList()
      });

    try
    {
      var currentUser = await _userManager.GetUserAsync(User);

      var post = await _postService.UpdatePost(id, model, currentUser);

      var postViewModel = new PostViewModel
      {
        Id = post.Id,
        Title = post.Title,
        Summary = post.Summary,
        Slug = post.Slug,
        Content = post.Content
      };

      return Ok(postViewModel);
    }
    catch (PostNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
    catch (AuthorizationException ex)
    {
      return Unauthorized(ex.Message);
    }
    catch (ImageUploadException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (Exception)
    {
      return BadRequest("An error occurred while processing the request.");
    }
  }

  [AllowAnonymous]
  [HttpGet]
  public async Task<ActionResult<IEnumerable<PostViewModel>>> GetPosts()
  {
    var posts = await _postService.GetPosts();
    return Ok(posts);
  }

  [AllowAnonymous]
  [HttpGet("{identifier}")]
  public async Task<ActionResult<PostViewModel>> GetPost(string identifier)
  {
    if (ObjectId.TryParse(identifier, out ObjectId objectId)) // Check if it's a valid ObjectId (ID)
    {
      var post = await _postService.GetPostById(objectId.ToString());
      if (post == null)
        return NotFound();

      return Ok(post);
    }
    else // If not a valid ObjectId, treat it as a slug
    {
      var post = await _postService.GetPostBySlug(identifier);
      if (post == null)
        return NotFound();

      return Ok(post);
    }
  }
}
