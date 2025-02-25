using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Dispute;

namespace GameTradeZone.Service.Interfaces
{
    public interface IDisputeService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> Add(AddDisputeModel model);
        Task<ApiResult> Update(UpdateDisputeModel model);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> GetAllByUserId(int id);
        Task<ApiResult> GetAllBySellerId(int id);
        Task<ApiResult> Reply(int id, string rep);
    }
}
