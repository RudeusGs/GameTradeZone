using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.HiredService;

namespace GameTradeZone.Service.Interfaces
{
    public interface IHiredServiceService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetAllByUserId(int id);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> GetAllByServiceID(int id);
        Task<ApiResult> ConfirmService(AcceptServiceModel model);
        Task<ApiResult> SendProof(ProofDoneService model);
    }
}
