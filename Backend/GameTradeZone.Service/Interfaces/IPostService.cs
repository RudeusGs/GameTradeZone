using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Interfaces
{
    public interface IPostService
    {
        Task<ApiResult> LikePost(int postId);
        Task<ApiResult> Comment(int postId, string content);
        Task<List<CommentData>> GetAllCommentsInPost(int postId);
        Task<ApiResult> DeleteComment(int commentId);
        Task<ApiResult> EditComment(int commentId, string content);
        Task<ApiResult> UnlikePost(int postId);

    }
}
