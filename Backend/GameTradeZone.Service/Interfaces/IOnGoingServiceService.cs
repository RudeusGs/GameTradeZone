using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.OngoingService;

namespace GameTradeZone.Service.Interfaces
{
    public interface IOnGoingServiceService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetAllByUserID(int id);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> ConfirmService(ConfirmServiceModel model);
        Task<ApiResult> GetAllByServiceID(int id);
        Task<ApiResult> GetNameServiceByServiceId(int id);
        Task<ApiResult> ConfirmExtension(int onGoingServiceId, bool approve, TimeSpan? approvedTime = null);
        Task<ApiResult> GetRemainingTime(int onGoingServiceId);
    }
}
