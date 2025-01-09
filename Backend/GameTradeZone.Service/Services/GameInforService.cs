using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.GameInfor;

namespace GameTradeZone.Service.Services
{
    public class GameInforService : ServiceBase, IGameInforService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        public GameInforService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService, IUserService userService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
        }

        public Task<ApiResult> Add(AddGameInforModel model)
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

        public Task<ApiResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> Update(UpdateGameInforModel model)
        {
            throw new NotImplementedException();
        }
    }
}
