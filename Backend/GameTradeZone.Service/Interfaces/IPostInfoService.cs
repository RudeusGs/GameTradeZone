using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTradeZone.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Interfaces
{
    public interface IPostInfoService
    {
        Task<PostInfo> CreatePost(string caption,int categoryId,string content, IFormFile? image);
        Task<List<PostInfo>> GetAllPosts();
        Task<bool> DeletePost(int postId, int userId);
        Task<PostInfo> GetPostById(int postId);
        Task<List<PostInfo>> GetAllPostByCategoryId(int categoryId);
    }
}
