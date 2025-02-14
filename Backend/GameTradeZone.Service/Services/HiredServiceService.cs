using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class HiredServiceService : ServiceBase, IHiredServiceService
    {
        public HiredServiceService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> ConfirmService(int id, string status)
        {
            var hiredService = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.Id == id);
            if (hiredService == null || hiredService.IsDelete == true) 
            {
                return new ApiResult { Message = "Không tìm thấy thành phần này!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newOnGoing = new OnGoingService
                {
                    Status = status,
                    UpdatedDate = DateTime.Now,
                };
                _dataContext.OnGoingServices.Update(newOnGoing);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult ();
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> Delete(int id)
        {
            var hiredService = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.Id == id);
            if(hiredService == null || hiredService.IsDelete == true)
            {
                return new ApiResult { Message = "Không tìm thấy thành phần này!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                hiredService.IsDelete = true;
                hiredService.DeleteDate = DateTime.Now;

                _dataContext.HiredServices.Update(hiredService);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult ();
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> GetAll()
        {
            var hiredService = await _dataContext.HiredServices.Where(x => x.IsDelete == false).ToListAsync();
            return new(hiredService);
        }

        public async Task<ApiResult> GetAllByServiceID(int id)
        {
            var hiredService = await _dataContext.HiredServices.Where(x => x.ServiceID == id && x.IsDelete == false).ToListAsync();
            return new(hiredService);
        }

        public async Task<ApiResult> GetAllByUserId(int id)
        {
            var hiredService = await _dataContext.HiredServices.Where(x => x.UserID == id && x.IsDelete == false).ToListAsync();
            return new(hiredService);
        }
    }
}
