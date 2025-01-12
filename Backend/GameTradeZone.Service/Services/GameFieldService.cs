using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.GameField;

namespace GameTradeZone.Service.Services
{
    public class GameFieldService : ServiceBase, IGameFieldService
    {
        public GameFieldService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public Task<ApiResult> Add(AddGameFieldModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetByGameInforID(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> Update(UpdateGameFieldModel model)
        {
            throw new NotImplementedException();
        }
    }
}
