using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.PurchasedAccount;

namespace GameTradeZone.Service.Interfaces
{
    public interface IPurchasedAccountService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetAllByUserID(int id);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> ComfirmAccount(ComfirmModel model);
        Task<ApiResult> EmailRequest();
        Task<ApiResult> EmailResponse(string email);
        Task<ApiResult> OTPRequest();
        Task<ApiResult> OTPResponse(string response);
    }
}
