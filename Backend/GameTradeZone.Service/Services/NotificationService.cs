using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;

namespace GameTradeZone.Service.Services
{
    public class NotificationService : ServiceBase, INotificationService
    {
        public NotificationService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public Task<ApiResult> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> GetAllByUserId(int UserId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> Read(int id)
        {
            throw new NotImplementedException();
        }
    }
}
