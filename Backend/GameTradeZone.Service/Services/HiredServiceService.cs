using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Services
{
    public class HiredServiceService : ServiceBase, IHiredServiceService
    {
        public HiredServiceService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
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

        public Task<ApiResult> GetAllByUserId(int id)
        {
            throw new NotImplementedException();
        }
    }
}
