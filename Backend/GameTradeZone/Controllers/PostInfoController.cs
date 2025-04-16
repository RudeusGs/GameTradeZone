using GameTradeZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<IActionResult> CreatePost([FromForm] string caption, [FromForm] int categoryId, [FromForm] string content, [FromForm] List<IFormFile>? images)
        {
            try
            {
                var post = await _postService.CreatePost(caption, categoryId, content, images);
                return Response(post);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            try
            {
                var post = await _postService.GetPostById(id);
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
                var success = await _postService.DeletePost(id);
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

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetAllPostByCategoryId(int categoryId)
        {
            try
            {
                var posts = await _postService.GetAllPostByCategoryId(categoryId);
                return Response(posts);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetAllPostByUserId(int userId)
        {
            try
            {
                var posts = await _postService.GetAllPostsByUserId(userId);
                return Response(posts);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpPost("update/{postId}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int postId, [FromForm] string caption, [FromForm] int categoryId, [FromForm] string content, [FromForm] List<IFormFile>? images)
        {
            try
            {
                var post = await _postService.UpdatePost(postId, caption, categoryId, content, images);
                return Response(post);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}