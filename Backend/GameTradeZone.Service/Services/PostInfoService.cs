using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Services
{
    public class PostInfoService : IPostInfoService
    {
        private readonly DataContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public PostInfoService(DataContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<PostInfo> CreatePost(int userId, string caption, string content, IFormFile? image)
        {
            string imageUrl = null;
            if (image != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(image);
            }

            var post = new PostInfo
            {
                UserId = userId,
                Caption = caption,
                Content = content,
                ImageUrl = imageUrl,
                CreatedDate = DateTime.UtcNow
            };

            await _context.PostInfos.AddAsync(post);
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
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
