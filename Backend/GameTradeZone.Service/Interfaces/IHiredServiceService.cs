using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface IHiredServiceService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetAllByUserId(int id);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> GetAllByServiceID(int id);
        Task<ApiResult> ConfirmService(int id, string status);
    }
}
