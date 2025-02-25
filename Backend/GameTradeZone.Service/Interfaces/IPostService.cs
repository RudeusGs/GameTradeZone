using GameTradeZone.Service.Models;
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
    }
}
