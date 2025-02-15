using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.PurchasedAccount;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace GameTradeZone.Service.Services
{
    public class PurchasedAccountService : ServiceBase, IPurchasedAccountService
    {
        public PurchasedAccountService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }

        public async Task<ApiResult> ComfirmAccount(ComfirmModel model)
        {
            var purchased = await _dataContext.PurchasedAccounts.FirstOrDefaultAsync(x => x.Id == model.Id);        
            if (purchased == null)
            {
                return new ApiResult { Message = "Không tìm thấy tài khoản này" };
            }
            var accountGame = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.Id == purchased.AccountGameId);
            if (purchased.IsDelete == true)
            {
                return new ApiResult { Message = "Thành phần này đã bị xóa!" };
            }
            var buyer = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == purchased.UserID);
            var seller = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == purchased.SellerID);
            if (seller == null || buyer == null || accountGame == null)
            {
                return new ApiResult { Message = "Không tìm thấy thông tin" };
            }

            using (var tran = await _dataContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.Status == "Từ chối")
                    {
                        purchased.StatusBuyer = "Đã từ chối";
                        purchased.StatusSeller = "Người mua từ chối";
                        purchased.Reason = model.Reason;
                        purchased.UpdatedDate = DateTime.Now;  
                        buyer.Balance += purchased.Price;
                        _dataContext.Users.Update(buyer);

                        _dataContext.PurchasedAccounts.Update(purchased);
                        await _dataContext.SaveChangesAsync();
                        await tran.CommitAsync();

                        return new ApiResult(purchased);
                    }
                    else if (model.Status == "Đồng ý")
                    {
                        accountGame.CustomerFeedback = model.Feedback;
                        purchased.StatusBuyer = "Mua thành công";
                        purchased.StatusSeller = "Thành công";
                        purchased.UpdatedDate = DateTime.Now;
                        seller.Balance += purchased.Price;
                        UpdateSellerLevel(seller, purchased.Price);
                        UpdateSellerLevel(buyer, purchased.Price);
                        _dataContext.AccountGames.Update(accountGame);
                        _dataContext.Users.Update(seller);
                        _dataContext.PurchasedAccounts.Update(purchased);
                        await _dataContext.SaveChangesAsync();
                        await tran.CommitAsync();

                        return new ApiResult(purchased);
                    }
                    else
                    {
                        return new ApiResult { Message = "Trạng thái không hợp lệ" };
                    }
                }
                catch (Exception e)
                {
                    await tran.RollbackAsync();
                    return new ApiResult { Message = $"Error: {e.Message}" };
                }
            }
        }

        private void UpdateSellerLevel(User seller, decimal? transactionAmount)
        {

            decimal[] thresholds = new decimal[]
            {
        100000m,  // level 0 -> 1
        300000m,  // level 1 -> 2
        500000m,  // level 2 -> 3
        700000m,  // level 3 -> 4
        1300000m, // level 4 -> 5
        1600000m, // level 5 -> 6
        2000000m, // level 6 -> 7
        2500000m, // level 7 -> 8
        3000000m, // level 8 -> 9
        4000000m  // level 9 -> 10
            };
            seller.Experience = seller.Level + transactionAmount;
            while (seller.Level < thresholds.Length && seller.Experience >= thresholds[seller.Level])
            {
                seller.Experience -= thresholds[seller.Level];
                seller.Level++;
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
                return new ApiResult();
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }

        }

        public Task<ApiResult> EmailRequest()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> EmailResponse(string email)
        {
            throw new NotImplementedException();
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

        public Task<ApiResult> OTPRequest()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> OTPResponse(string response)
        {
            throw new NotImplementedException();
        }
    }
}
