using Azure;
using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<CommentData>> GetAllCommentsInPost(int postId)
        {
            Console.WriteLine($"📌 Fetching comments for Post ID: {postId}"); // Debug log

            var comments = await _context.CommentDatas
                .Where(c => c.PostId == postId) // Lọc theo postId
                .Include(c => c.User) // Lấy thông tin người dùng
                .Select(c => new CommentData
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate,
                    User = new User
                    {
                        Id = c.User.Id,
                        UserName = c.User.UserName,
                        FullName = c.User.FullName,
                        Avatar = c.User.Avatar
                    }
                })
                .ToListAsync();

            Console.WriteLine($"✅ Found {comments.Count} comments for Post ID: {postId}"); // Debug log
            return comments;
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
            post.CommentsCount += 1;
            _context.CommentDatas.Add(newComment);
            await _context.SaveChangesAsync();

            return new ApiResult
            {
                Message = "Comment added successfully"
            };
        }

        public async Task<ApiResult> DeleteComment(int commentId)
        {
            var comment = await _context.CommentDatas.FindAsync(commentId);
            if (comment == null)
                return new ApiResult { Message = "Comment not found" };

            _context.CommentDatas.Remove(comment);

            var post = await _context.PostInfos.FindAsync(comment.PostId);
            if (post != null && post.CommentsCount > 0)
                post.CommentsCount -= 1;

            await _context.SaveChangesAsync();

            return new ApiResult { Message = "Comment deleted successfully" };
        }
        public async Task<ApiResult> EditComment(int commentId, string content)
        {
            var comment = await _context.CommentDatas.FindAsync(commentId);
            if (comment == null)
                return new ApiResult { Message = "Comment not found" };

            comment.Content = content;
            await _context.SaveChangesAsync();

            return new ApiResult { Message = "Comment updated successfully" };
        }

        public async Task<ApiResult> UnlikePost(int postId)
        {
            var post = await _context.PostInfos.FindAsync(postId);
            if (post == null)
            {
                return new ApiResult { Message = "Post not found" };
            }

            if (post.LikesCount > 0) post.LikesCount -= 1;

            await _context.SaveChangesAsync();
            return new ApiResult { Message = "Post unliked" };
        }
    }
}
