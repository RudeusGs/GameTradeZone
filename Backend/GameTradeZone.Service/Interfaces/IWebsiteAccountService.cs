using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface IWebsiteAccountService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
    }
}
