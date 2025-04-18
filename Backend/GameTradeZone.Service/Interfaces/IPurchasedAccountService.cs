using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.PurchasedAccount;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Interfaces
{
    public interface IPurchasedAccountService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetAllByUserID(int id);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> ComfirmAccount(ComfirmModel model);
        Task<ApiResult> EmailRequest(int id);
        Task<ApiResult> EmailResponse(int id, string email);
        Task<ApiResult> OTPRequest(int id);
        Task<ApiResult> OTPResponse(int id, string response);
        Task<ApiResult> GetDontConfirm();
    }
}
