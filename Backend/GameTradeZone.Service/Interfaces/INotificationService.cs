using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface INotificationService
    {
        Task<ApiResult> GetAllByUserId(int UserId);
        Task<ApiResult> Read(int id);
        Task<ApiResult> Delete(int id);
    }
}
