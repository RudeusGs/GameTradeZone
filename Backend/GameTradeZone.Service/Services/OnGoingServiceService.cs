using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Services
{
    public class OnGoingServiceService : ServiceBase, IOnGoingServiceService
    {
        public OnGoingServiceService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public Task<ApiResult> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetAllByUserID(int id)
        {
            throw new NotImplementedException();
        }
    }
}
