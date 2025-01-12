using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Service;

namespace GameTradeZone.Service.Interfaces
{
    public interface IServiceService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> Add(AddServiceModel model);
        Task<ApiResult> Update(UpdateServiceModel model);
        Task<ApiResult> Delete(int id);
    }
}
