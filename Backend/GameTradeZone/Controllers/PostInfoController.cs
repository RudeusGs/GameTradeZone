using GameTradeZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameTradeZone.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostInfoController : BaseController
    {
        private readonly IPostInfoService _postService;
        public PostInfoController(IPostInfoService postService)
        {
            _postService = postService;
        }

       
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromForm] string caption, [FromForm] string content, [FromForm] IFormFile? image)
        {
            try
            {
                var post = await _postService.CreatePost(caption, content, image);
                return Response(post);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _postService.GetAllPosts();
                return Response(posts);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
       
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Response(new { Message = "Người dùng chưa đăng nhập!" });
                }
                var userId = int.Parse(userIdClaim);
                var success = await _postService.DeletePost(id, userId);
                if (!success)
                {
                    return Response(new { Message = "Không thể xóa bài viết!" });
                }

                return Response(new { Message = "Bài viết đã được xóa!" });
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
