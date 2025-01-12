using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface IOnGoingServiceService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetAllByUserID(int id);
        Task<ApiResult> Delete(int id);
    }
}
