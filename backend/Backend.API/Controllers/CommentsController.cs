using Backend.API.Helpers;
using Backend.API.Models;
using Backend.Application.Models;
using Backend.Application.Services;
using Backend.Application.Validators;
using Backend.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

public class CommentsController : BaseApiController
{
    private readonly ICommentsService _commentsService;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;

    public CommentsController(ICommentsService commentsService, UserManager<User> userManager, IConfiguration config)
    {
        _commentsService = commentsService;
        _userManager = userManager;
        _config = config;
    }

    [Authorize]
    [HttpPost("{identifier}")]
    [ProducesResponseType(typeof(CommentViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddCommentToPost(string identifier, [FromBody] CommentInputModel comment)
    {
        var validationResult = ValidateModel<CommentInputModelValidator, CommentInputModel>(comment);

        if (!validationResult.IsValid)
            return BadRequest(new ApiResponse<CommentViewModel>
            {
                Success = false,
                Errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList()
            });

        var email = AutoLoginHelper.GetEmailFromValidToken(_config, comment.token);
        if (email == null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Unauthorized();
        }

        Comment addedComment;

        if (Guid.TryParse(identifier, out _))
        {
            // If identifier is a valid Guid, assume it's an ID
            addedComment = await _commentsService.AddCommentToPostById(identifier, comment, user);
        }
        else
        {
            // If identifier is not a valid Guid, assume it's a slug
            addedComment = await _commentsService.AddCommentToPostBySlug(identifier, comment, user);
        }

        var commentViewModel = new CommentViewModel
        {
            Id = addedComment.Id,
            Content = addedComment.Content,
            Author = addedComment.Author,
            CreatedAt = addedComment.CreatedAt
        };
        return Ok(commentViewModel);
    }

    [HttpGet("{identifier}")]
    [ProducesResponseType(typeof(List<CommentViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCommentsForPost(string identifier)
    {
        IEnumerable<Comment> comments;

        if (Guid.TryParse(identifier, out _))
        {
            // If identifier is a valid Guid, assume it's an ID
            comments = await _commentsService.GetCommentsForPostById(identifier);
        }
        else
        {
            // If identifier is not a valid Guid, assume it's a slug
            comments = await _commentsService.GetCommentsForPostBySlug(identifier);
        }

        List<CommentViewModel> commentsViewModel = comments.Select(c => new CommentViewModel
        {
            Id = c.Id,
            Content = c.Content,
            Author = c.Author,
            CreatedAt = c.CreatedAt
        }).ToList();

        return Ok(commentsViewModel);
    }
}
