using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTradeZone.Domain.Entities;
using Microsoft.AspNetCore.Http;
using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface IPostInfoService
    {
        Task<PostInfo> CreatePost(string caption, int categoryId, string content, List<IFormFile>? images);
        Task<List<PostInfo>> GetAllPosts();
        Task<bool> DeletePost(int postId);
        Task<PostInfo> GetPostById(int postId);
        Task<List<PostInfo>> GetAllPostByCategoryId(int categoryId);
        Task<List<PostInfo>> GetAllPostsByUserId(int userId);
        Task<PostInfo> UpdatePost(int postId, string caption, int categoryId, string content, List<IFormFile>? images);
        Task<ApiResult> GetPostCount();
    }
}
