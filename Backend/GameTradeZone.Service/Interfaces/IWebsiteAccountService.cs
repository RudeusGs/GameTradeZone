using GameTradeZone.Service.Models;
using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Interfaces
{
    public interface IWebsiteAccountService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> UpdateRoles(int id, List<string> newRoles);
        Task<ApiResult> BlockAccount(int id);
        Task<ApiResult> UpdateUserAvatar(IFormFile? image);
    }
}
