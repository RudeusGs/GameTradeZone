using GameTradeZone.Service.Models.GameInfor;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.LevelIcon;

namespace GameTradeZone.Service.Interfaces
{
    public interface ILevelIconService
    {
        Task<ApiResult> GetAll();
        Task<ApiResult> GetById(int id);
        Task<ApiResult> Add(AddLevelIconModel model);
        Task<ApiResult> Update(UpdateGameInforModel model);
        Task<ApiResult> Delete(int id);
    }
}
