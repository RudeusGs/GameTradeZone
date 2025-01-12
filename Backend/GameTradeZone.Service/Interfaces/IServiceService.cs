using GameTradeZone.Service.Models.Dispute;
using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Interfaces
{
    public interface IServiceService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> Add(AddDisputeModel model);
        Task<ApiResult> Update(UpdateDisputeModel model);
        Task<ApiResult> Delete(int id);
    }
}
