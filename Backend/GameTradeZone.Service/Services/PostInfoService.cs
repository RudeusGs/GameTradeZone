using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Services
{
    public class PostInfoService : IPostInfoService
    {
        private readonly DataContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostInfoService(DataContext context, CloudinaryService cloudinaryService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _httpContextAccessor = httpContextAccessor;

        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)
                       ?? _httpContextAccessor.HttpContext?.User.FindFirst("unique_name");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return null; 
            return userId;
        }

        public async Task<PostInfo> CreatePost(string caption, int categoryId, string content, IFormFile? image)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("User is not logged in");
            string imageUrl = null;
            if (image != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(image);
            }

            var post = new PostInfo
            {
                UserId = userId.Value,
                Caption = caption,
                CategoryId = categoryId,
                Content = content,
                ImageUrl = imageUrl,
                CreatedDate = DateTime.UtcNow
            };

            await _context.PostInfos.AddAsync(post);
            var category = await _context.ForumsCategories.FindAsync(categoryId);
            if (category != null)
            {
                category.PostCount += 1;
            }
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<List<PostInfo>> GetAllPosts()
        {
            return await _context.PostInfos.Include(p => p.User).ToListAsync();
        }

        public async Task<bool> DeletePost(int postId, int userId)
        {
            var post = await _context.PostInfos.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);

            if (post == null)
            {
                return false;
            }

            _context.PostInfos.Remove(post);
            var category = await _context.ForumsCategories.FindAsync(post.CategoryId);
            if (category != null)
            {
                category.PostCount -= 1;
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<PostInfo> GetPostById(int postId)
        {
            var post = await _context.PostInfos
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == postId);
            return post ?? throw new KeyNotFoundException($"Post with ID {postId} not found.");
        }

        public async Task<List<PostInfo>> GetAllPostByCategoryId(int categoryId)
        {
            return await _context.PostInfos
                .Include(p => p.User)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
