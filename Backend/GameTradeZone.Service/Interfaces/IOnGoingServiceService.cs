using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface IOnGoingServiceService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetAllByUserID(int id);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> ConfirmService(int id, string status);
        Task<ApiResult> GetAllByServiceID(int id);
    }
}
