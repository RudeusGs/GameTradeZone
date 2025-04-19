using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
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
        private readonly IUserService _userService;

        public PostInfoService(DataContext context, CloudinaryService cloudinaryService, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;

        }

        public async Task<PostInfo> CreatePost(string caption, int categoryId, string content, List<IFormFile>? images)
        {
            var userId = _userService.UserId;
            string? imageUrls = null;
            if (images != null && images.Count > 1)
            {
                var uploadedImages = await _cloudinaryService.UploadMutilImage(images);
                if (uploadedImages.Any())
                {
                    imageUrls = string.Join(";", uploadedImages);
                }
            }
            else if (images != null && images.Count == 1)
            {
                var uploadedImage = await _cloudinaryService.UploadImageAsync(images[0]);
                imageUrls = uploadedImage;
            }

            var post = new PostInfo
            {
                UserId = userId,
                Caption = caption,
                CategoryId = categoryId,
                Content = content,
                ImageUrl = imageUrls,
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

        public async Task<bool> DeletePost(int postId)
        {
            var userId = _userService.UserId;
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

        public async Task<List<PostInfo>> GetAllPostsByUserId(int userId)
        {
            return await _context.PostInfos
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<PostInfo> UpdatePost(int postId, string caption, int categoryId, string content, List<IFormFile>? images)
        {
            var userId = _userService.UserId;
            var post = await _context.PostInfos.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
            if (post == null)
                throw new KeyNotFoundException($"Post with ID {postId} not found or you do not have permission to update it.");

            post.Caption = caption;
            post.CategoryId = categoryId;
            post.Content = content;

            if (images != null && images.Count > 0)
            {
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    var existingImages = post.ImageUrl.Split(';');
                    foreach (var imageUrl in existingImages)
                    {
                        await _cloudinaryService.DeleteImageAsync(imageUrl);
                    }
                }

                string? imageUrls = null;
                if (images.Count > 1)
                {
                    var uploadedImages = await _cloudinaryService.UploadMutilImage(images);
                    if (uploadedImages.Any())
                    {
                        imageUrls = string.Join(";", uploadedImages);
                    }
                }
                else if (images.Count == 1)
                {
                    var uploadedImage = await _cloudinaryService.UploadImageAsync(images[0]);
                    imageUrls = uploadedImage;
                }
                post.ImageUrl = imageUrls;
            }

            _context.PostInfos.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }
        public async Task<ApiResult> GetPostCount()
        {
            var postCount = await _context.PostInfos.CountAsync();
            return new ApiResult
            {
                Message = "Post count retrieved successfully.",
                Data = postCount
            };
        }
    }
}
