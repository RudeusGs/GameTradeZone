using Azure;
using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PostService(DataContext  dataContext, IHttpContextAccessor httpContextAccessor)
        {
             _context = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public int? GetPostID()
        {
            return _context.PostInfos.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault();
        }
        private int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)
                       ?? _httpContextAccessor.HttpContext?.User.FindFirst("unique_name");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return null;
            return userId;
        }
        public async Task<ApiResult> LikePost(int postId)
        {
            var post = await _context.PostInfos.FindAsync(postId);

            if (post == null)
            {
                return new ApiResult
                {                
                    Message = "Post not found"
                };
            }

            post.LikesCount += 1; 
            await _context.SaveChangesAsync();
            return new ApiResult
            {
                Message = "Post liked"
            };
        }
        public async Task<ApiResult> Comment(int postId, string content)
        {
            var post = await _context.PostInfos.FindAsync(postId);
            var userId = GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("User is not logged in");

            if (post == null)
            {
                return new ApiResult
                {
                    Message = "Post not found"
                };
            }

            var newComment = new CommentData
            {
                PostId = postId,
                UserId= userId.Value,
                Content = content,
                CreatedDate = DateTime.UtcNow
            };

            _context.CommentDatas.Add(newComment);
            await _context.SaveChangesAsync();

            return new ApiResult
            {
                Message = "Comment added successfully"
            };
        }
    }
}
