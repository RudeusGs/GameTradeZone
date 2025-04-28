using GameTradeZone.Service.Models.HiredService;
using GameTradeZone.Service.Models;

public interface IHiredServiceService
{
    Task<ApiResult> GetAll();
    Task<ApiResult> GetAllByUserId(int id);
    Task<ApiResult> Delete(int id);
    Task<ApiResult> GetAllByServiceID(int id);
    Task<ApiResult> ConfirmService(AcceptServiceModel model);
    Task<ApiResult> SendProof(ProofDoneService model);
    Task<ApiResult> DoneService(int id);
    Task<ApiResult> ExtendTime(int Id, TimeSpan extensionTime);
}