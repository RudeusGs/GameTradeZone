using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class NotificationService : ServiceBase, INotificationService
    {
        public NotificationService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> Delete(int id)
        {
            var noTi = await _dataContext.Notifications.FirstOrDefaultAsync(x => x.Id == id);
            if (noTi == null || noTi.IsDelete == true) 
            {
                return new ApiResult { Message = "Thông báo này không tồn tại hoặc đã bị xóa" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                noTi.IsDelete = true;
                noTi.DeleteDate = DateTime.Now;
                _dataContext.Notifications.Update(noTi);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = "Xóa thành công" };
            }
            catch(Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };   
            }
        }

        public async Task<ApiResult> GetAllByUserId(int UserId)
        {
            var noTi = await _dataContext.Notifications.Where(x => x.UserID == UserId && x.IsDelete == false).ToListAsync();
            return new(noTi);
        }

        public async Task<ApiResult> Read(int id)
        {
            var noTi = await _dataContext.Notifications.FirstOrDefaultAsync(x => x.Id == id);
            if (noTi == null || noTi.IsDelete == true)
            {
                return new ApiResult { Message = "Thông báo này không tồn tại hoặc đã bị xóa" };
            }
            if(noTi.IsRead == true)
            {
                return new ApiResult { Message = "Thông báo này đã được đọc" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                noTi.IsRead = true;
                noTi.UpdatedDate = DateTime.Now;
                _dataContext.Notifications.Update(noTi);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = "Đọc thành công" };
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }
    }
}
