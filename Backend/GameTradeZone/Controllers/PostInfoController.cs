using GameTradeZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameTradeZone.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostInfoController : Controller
    {
        private readonly IPostInfoService _postService;
        public PostInfoController(IPostInfoService postService)
        {
            _postService = postService;
        }

        // API tạo bài viết
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromForm] string caption,[FromForm] string content, [FromForm] IFormFile? image)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var post = await _postService.CreatePost(userId,caption ,content, image);
            return Ok(post);
        }


        // API lấy danh sách bài viết mới nhất
        [HttpGet("latest")]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _postService.GetAllPosts();
            return Ok(posts);
        }

        // API xóa bài viết
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return Unauthorized(new { Message = "Người dùng chưa đăng nhập!" });

            var success = await _postService.DeletePost(id, userId);
            if (!success)
                return NotFound(new { Message = "Không thể xóa bài viết!" });

            return Ok(new { Message = "Bài viết đã được xóa!" });
        }
    }
}
