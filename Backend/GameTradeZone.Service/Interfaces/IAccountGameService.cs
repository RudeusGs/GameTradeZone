using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.AccountGame;

namespace GameTradeZone.Service.Interfaces
{
    public interface IAccountGameService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> Add(AddAccountGameModel model);
        Task<ApiResult> Update(UpdateAccountGameModel model);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> Buy(BuyAccountGameModel model);
        Task<ApiResult> Accept(int id);
        Task<ApiResult> GetAllByUserID(int UserId);
        Task<ApiResult> GetInforUser();
        Task<ApiResult> BargainPrice(BargainAccountGameModel model);

    }
}
