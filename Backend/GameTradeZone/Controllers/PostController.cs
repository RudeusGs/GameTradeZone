using GameTradeZone.Controllers;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class PostInfoController : BaseController
{
    private readonly IPostService _postService;

    public PostInfoController(IPostService postService)
    {
        _postService = postService;
    }
    [Authorize]
    [HttpPost("like/{postId}")]
    public async Task<IActionResult> LikePost(int postId)
    {
        try
        {
            var result = await _postService.LikePost(postId);
            return Response(result);
        }
        catch (Exception e)
        {
            return Response(e.Message, 500);
        }
    }
    [Authorize]
    [HttpPost("comment")]
    public async Task<IActionResult> Comment([FromBody] CommentRequest request)
    {
        try
        {
            var result = await _postService.Comment(request.PostId, request.Content);
            return Response(result);
        }
        catch (Exception e)
        {
            return Response(e.Message, 500);
        }
    }
    [HttpGet("comments/{postId}")]
    public async Task<IActionResult> GetAllCommentsInPost(int postId)
    {
        try
        {
            Console.WriteLine($"📌 Received Post ID: {postId}"); // Debug log
            var comments = await _postService.GetAllCommentsInPost(postId);
            return Response(comments);
        }
        catch (Exception e)
        {
            return Response(e.Message, 500);
        }
    }
    [Authorize]
    [HttpDelete("comment/{commentId}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        try
        {
            var result = await _postService.DeleteComment(commentId);
            return Response(result);
        }
        catch (Exception e)
        {
            return Response(e.Message, 500);
        }
    }
    [Authorize]
    [HttpPost("comment/{commentId}")]
    public async Task<IActionResult> EditComment(int commentId, [FromBody] CommentRequest request)
    {
        try
        {
            var result = await _postService.EditComment(commentId, request.Content);
            return Response(result);
        }
        catch (Exception e)
        {
            return Response(e.Message, 500);
        }
    }
    [Authorize]
    [HttpPost("unlike/{postId}")]
    public async Task<IActionResult> UnlikePost(int postId)
    {
        try
        {
            var result = await _postService.UnlikePost(postId);
            return Response(result);
        }
        catch (Exception e)
        {
            return Response(e.Message, 500);
        }
    }
}
