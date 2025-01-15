using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class PurchasedAccountService : ServiceBase, IPurchasedAccountService
    {
        public PurchasedAccountService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> ComfirmAccount(int id, string status)
        {
            var purChased = await _dataContext.PurchasedAccounts.FirstOrDefaultAsync(x => x.Id == id);
            if (purChased == null)
            {
                return new ApiResult { Message = "Không tìm thấy tài khoản này" };
            }
            if(purChased.IsDelete == true)
            {
                return new ApiResult { Message = "Thành phần này đã bị xóa!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newAccount = new AccountGame 
                { 
                    Status = status,
                    UpdatedDate = DateTime.Now,
                };
                _dataContext.AccountGames.Update(newAccount);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = "Xác nhận thành công!" };
            }
            catch(Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }

        }

        public async Task<ApiResult> Delete(int id)
        {
            var purChased = await _dataContext.PurchasedAccounts.FirstOrDefaultAsync(x => x.Id == id);
            if(purChased == null || purChased.IsDelete == true)
            {
                return new ApiResult { Message = "Không tìm thấy hoặc đã bị xóa!" };
            }
            var tran =await _dataContext.Database.BeginTransactionAsync();
            try
            {
                purChased.IsDelete = true;
                purChased.DeleteDate = DateTime.Now;
                _dataContext.PurchasedAccounts.Update(purChased);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = "Xóa thành công!" };
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }

        }

        public async Task<ApiResult> GetAll()
        {
            var purChased = await _dataContext.PurchasedAccounts.Where(x => x.IsDelete == false).ToListAsync();
            return new(purChased);
        }

        public async Task<ApiResult> GetAllByUserID(int id)
        {
            var purChased = await _dataContext.PurchasedAccounts.Where(x => x.UserID == id && x.IsDelete == false).ToListAsync();
            return new(purChased);
        }
    }
}
