using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace GameTradeZone.Service.Services
{
    public class OnGoingServiceService : ServiceBase, IOnGoingServiceService
    {
        public OnGoingServiceService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> ConfirmService(int id, string status)
        {
            var onGoingService = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.Id == id);
            if(onGoingService == null) 
            {
                return new ApiResult { Message = "Không tìm thấy dịch vụ này!" };
            }
            if(onGoingService.IsDelete == true)
            {
                return new ApiResult { Message = "Dịch vụ này đã bị xóa" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newHiredService = new HiredService
                {
                    Status = status,
                    UpdatedDate = DateTime.Now,
                };
                _dataContext.HiredServices.Update(newHiredService);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = "Thay đổi trạng thái thành công!" };
            }
            catch(Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> Delete(int id)
        {
            var onGoing = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.Id == id);
            if(onGoing == null)
            {
                return new ApiResult { Message = "Không thể xóa, không thể tìm thấy thành phần này" };
            }
            if(onGoing.IsDelete == true)
            {
                return new ApiResult { Message = "Thành phần này đã được xóa trước đó" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                onGoing.IsDelete = true;
                onGoing.DeleteDate = DateTime.Now;

                _dataContext.OnGoingServices.Update(onGoing);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = "Xóa thành công!" };
            }
            catch(Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> GetAll()
        {
            var onGoing = await _dataContext.OnGoingServices.Where(x => x.IsDelete == false).ToListAsync();
            return new(onGoing);
        }

        public async Task<ApiResult> GetAllByServiceID(int id)
        {
            var onGoing = await _dataContext.OnGoingServices.Where(x => x.ServiceID == id && x.IsDelete == false).ToListAsync();
            return new(onGoing);
        }

        public async Task<ApiResult> GetAllByUserID(int id)
        {
            var onGoing = await _dataContext.OnGoingServices.Where(x => x.UserID == id && x.IsDelete == false).ToListAsync();
            return new(onGoing);
        }
    }
}
