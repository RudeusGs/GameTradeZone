using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface IPurchasedAccountService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetAllByUserID(int id);
        Task<ApiResult> Delete(int id);
    }
}
