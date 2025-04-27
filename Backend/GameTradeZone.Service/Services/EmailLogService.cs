using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class EmailLogService : ServiceBase, IEmailLogService
    {
        public EmailLogService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> GetAll()
        {
            var result = await _dataContext.EmailLogs.ToListAsync();
            return new(result);
        }
    }
}
