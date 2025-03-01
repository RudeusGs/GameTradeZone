using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class WebsiteAccountService : ServiceBase, IWebsiteAccountService
    {
        public WebsiteAccountService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> GetAll()
        {
            var result = await _dataContext.Users.ToListAsync();
            return new(result);
        }

        public async Task<ApiResult> GetById(int id)
        {
            var result = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            return new(result);
        }
    }
}
