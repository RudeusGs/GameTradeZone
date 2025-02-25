using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.OngoingService;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace GameTradeZone.Service.Services
{
    public class OnGoingServiceService : ServiceBase, IOnGoingServiceService
    {
        public OnGoingServiceService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> ConfirmService(ConfirmServiceModel model)
        {
            var onGoingService = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.Id == model.Id);
            if(onGoingService == null) 
            {
                return new ApiResult { Message = "Không tìm thấy dịch vụ này!" };
            }
            if(onGoingService.IsDelete == true)
            {
                return new ApiResult { Message = "Dịch vụ này đã bị xóa" };
            }
            var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.Id == onGoingService.ServiceID);
            if (service == null)
            {
                return new ApiResult { Message = "Dịch vụ không tồn tại" };
            }
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == service.CreaterID);
            if(user == null)
            {
                return new ApiResult { Message = "User không tồn tại!" };
            }
            var hiredService = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.ServiceID == onGoingService.ServiceID);
            if(hiredService == null)
            {
                return new ApiResult { Message = "Không tồn tại!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                if(model.Status == "Từ chối")
                {
                    hiredService.Reason = model.Reason;
                    hiredService.Status = "Từ chối";
                }
                else if(model.Status == "Đồng ý")
                {
                    hiredService.Status = "Thành công";
                    decimal serviceAmount = service.ServicePrice * 0.94m;
                    user.Balance += service.ServicePrice;
                    _dataContext.Users.Update(user);
                    await _dataContext.SaveChangesAsync();
                    await tran.CommitAsync();
                }
                else
                {
                    return new ApiResult { Message = "Trạng thái không đúng" };
                }               

                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult ();
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
                return new ApiResult ();
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
