using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;

namespace GameTradeZone.Service.Services
{
    public class ServiceBase
    {
        protected readonly DataContext _dataContext;
        protected readonly DateTime _now = DateTime.UtcNow.AddHours(7);
        protected readonly IUserService _userService;
        protected readonly string _userName;
        public ServiceBase(DataContext dataContext, IUserService userService)
        {
            _dataContext = dataContext;
            _userService = userService;
            _userName = _userService.UserName;
        }
    }
}
