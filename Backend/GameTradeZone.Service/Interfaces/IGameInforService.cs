using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.GameInfor;

namespace GameTradeZone.Service.Interfaces
{
    public interface IGameInforService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> Add(AddGameInforModel model);
        Task<ApiResult> Update(UpdateGameInforModel model);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> GetByGameFieldID(int id);
    }
}
