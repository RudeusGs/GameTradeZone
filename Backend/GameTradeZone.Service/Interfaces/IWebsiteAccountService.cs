using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface IWebsiteAccountService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> UpdateRoles(int id, List<string> newRoles);
        Task<ApiResult> BlockAccount(int id);
    }
}
