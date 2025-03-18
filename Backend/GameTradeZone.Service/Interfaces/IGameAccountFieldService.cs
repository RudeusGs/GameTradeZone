using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.GameAccountField;

namespace GameTradeZone.Service.Interfaces
{
    public interface IGameAccountFieldService
    {
        Task<ApiResult> Add(AddGameAccountFieldModel model);
        Task<ApiResult> GetByID(int GameInforId, int GameFieldId);

    }
}
