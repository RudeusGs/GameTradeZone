using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.GameField;

namespace GameTradeZone.Service.Interfaces
{
    public interface IGameFieldService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> Add(AddGameFieldModel model);
        Task<ApiResult> Update(UpdateGameFieldModel model);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> GetByGameInforID(int id);
    }
}
