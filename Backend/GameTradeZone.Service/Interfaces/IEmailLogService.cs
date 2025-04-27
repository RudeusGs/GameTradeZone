using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface IEmailLogService
    {
        Task<ApiResult> GetAll();
    }
}
