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
            if (onGoingService == null)
            {
                return new ApiResult { Message = "Không tìm thấy dịch vụ này!" };
            }
            if (onGoingService.IsDelete == true)
            {
                return new ApiResult { Message = "Dịch vụ này đã bị xóa" };
            }
            var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.Id == onGoingService.ServiceID);
            if (service == null)
            {
                return new ApiResult { Message = "Dịch vụ không tồn tại" };
            }
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == service.CreaterID);
            if (user == null)
            {
                return new ApiResult { Message = "User không tồn tại!" };
            }
            var hiredService = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.Id == onGoingService.Id);
            if (hiredService == null)
            {
                return new ApiResult { Message = "Không tồn tại!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                if (model.Status == "Từ chối")
                {
                    hiredService.Reason = model.Reason;
                    hiredService.Status = "Từ chối";
                    onGoingService.Status = "Bạn đã từ chối";
                    _dataContext.HiredServices.Update(hiredService);
                    _dataContext.OnGoingServices.Update(onGoingService);
                    await _dataContext.SaveChangesAsync();
                    await tran.CommitAsync();
                    return new ApiResult(hiredService);
                }
                else if (model.Status == "Đồng ý")
                {
                    hiredService.Status = "Thành công";
                    onGoingService.Status = "Hoàn tất";
                    decimal serviceAmount = service.ServicePrice * 0.94m;
                    hiredService.FeedBack = model.FeedBack;
                    user.Balance += service.ServicePrice;
                    _dataContext.Users.Update(user);
                    _dataContext.HiredServices.Update(hiredService);
                    _dataContext.OnGoingServices.Update(onGoingService);
                    await _dataContext.SaveChangesAsync();
                    await tran.CommitAsync();
                    return new ApiResult(hiredService);
                }
                else
                {
                    return new ApiResult { Message = "Trạng thái không đúng" };
                }
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> Delete(int id)
        {
            var onGoing = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.Id == id);
            if (onGoing == null)
            {
                return new ApiResult { Message = "Không thể xóa, không thể tìm thấy thành phần này" };
            }
            if (onGoing.IsDelete == true)
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
                return new ApiResult();
            }
            catch (Exception e)
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

        public async Task<ApiResult> GetNameServiceByServiceId(int id)
        {
            var onGoing = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.ServiceID == id);
            if (onGoing == null)
            {
                return new ApiResult { Message = "Không tìm thấy thành phần này" };
            }
            var serviceName = await _dataContext.Services.Where(x => x.Id == onGoing.ServiceID).ToListAsync();
            return new(serviceName);
        }

        public async Task<ApiResult> ConfirmExtension(int onGoingServiceId, bool approve, TimeSpan? approvedTime = null)
        {
            var onGoingService = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.Id == onGoingServiceId && x.IsDelete == false);
            if (onGoingService == null)
            {
                return new ApiResult { Message = "Không tìm thấy dịch vụ này" };
            }
            var hiredService = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.OnGoingServiceId == onGoingServiceId && x.IsDelete == false);
            if (hiredService == null || !hiredService.ExtensionRequested)
            {
                return new ApiResult { Message = "Không có yêu cầu gia hạn cho dịch vụ này" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                if (approve)
                {
                    hiredService.ExtensionApproved = true;
                    hiredService.ApprovedExtensionTime = approvedTime ?? hiredService.RequestedExtensionTime;
                    onGoingService.EndTime += hiredService.ApprovedExtensionTime;
                }
                else
                {
                    hiredService.ExtensionApproved = false;
                    hiredService.ExtensionRequested = false;
                }
                _dataContext.HiredServices.Update(hiredService);
                _dataContext.OnGoingServices.Update(onGoingService);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = approve ? "Yêu cầu gia hạn đã được chấp nhận" : "Yêu cầu gia hạn đã bị từ chối" };
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> GetRemainingTime(int onGoingServiceId)
        {
            var onGoingService = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.Id == onGoingServiceId && x.IsDelete == false);
            if (onGoingService == null || onGoingService.EndTime == null)
            {
                return new ApiResult { Message = "Không tìm thấy dịch vụ hoặc dịch vụ chưa bắt đầu" };
            }
            var remainingTime = onGoingService.EndTime - DateTime.UtcNow;
            if (remainingTime <= TimeSpan.Zero)
            {
                return new ApiResult { Message = "Dịch vụ đã hết thời gian" };
            }
            return new ApiResult { Data = remainingTime };
        }
    }
}