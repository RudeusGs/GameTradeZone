using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.PurchasedAccount;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace GameTradeZone.Service.Services
{
    public class PurchasedAccountService : ServiceBase, IPurchasedAccountService
    {
        public PurchasedAccountService(DataContext dataContext, IUserService userService) : base(dataContext, userService)
        {
        }
        public async Task<ApiResult> GetDontConfirm()
        {
            var purchased = await _dataContext.PurchasedAccounts.FirstOrDefaultAsync(x => x.StatusBuyer == "Chưa xác nhận" && x.SellerID == _userService.UserId);
            return new(purchased);
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
                        decimal sellerAmount = purchased.Price * 0.93m;
                        seller.Balance += sellerAmount;
                        UpdateSellerLevel(seller, (long)purchased.Price);
                        UpdateSellerLevel(buyer, (long)purchased.Price);
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

        private void UpdateSellerLevel(User seller, long? transactionAmount)
        {

            long[] thresholds = new long[]
            {
        100000,  // level 0 -> 1
        300000,  // level 1 -> 2
        500000,  // level 2 -> 3
        700000,  // level 3 -> 4
        1300000, // level 4 -> 5
        1600000, // level 5 -> 6
        2000000, // level 6 -> 7
        2500000, // level 7 -> 8
        3000000, // level 8 -> 9
        4000000  // level 9 -> 10
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

        public async Task<ApiResult> EmailRequest(int id)
        {
            var purchased = await _dataContext.PurchasedAccounts.FirstOrDefaultAsync(x => x.Id == id);
            if (purchased == null || purchased.IsDelete == true)
            {
                return new ApiResult { Message = "Không tìm thấy giao dịch này" };
            }
            if(purchased.StatusBuyer == "Đã từ chối")
            {
                return new ApiResult { Message = "Bạn đã từ chối tài khoản này không thể yêu cầu gửi thông tin" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newNoti = new Notification
                {
                    TypeNoti = "Yêu cầu gửi gmail",
                    Content = $"Giao dịch tài khoản mã số {purchased.Id}: đã yêu cầu gửi thông tin(tài khoản gmail, số điện thoại,...) đăng ký tài khoản trò chơi, để đổi mật khẩu",
                    SenderID = _userService.UserId,
                    UserID = purchased.SellerID,
                    IsRead = false,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                };
                purchased.Email = "Đang chờ";
                purchased.OTPEmail = "Đang chờ";
                _dataContext.PurchasedAccounts.Update(purchased);
                _dataContext.Notifications.Add(newNoti);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Gửi email thất bại: {ex.Message}" };
            }
        }

        public async Task<ApiResult> EmailResponse(int id, string email)
        {
            var purchased = await _dataContext.PurchasedAccounts.FirstOrDefaultAsync(x => x.Id == id);
            if (purchased == null || purchased.IsDelete == true)
            {
                return new ApiResult { Message = "Không tìm thấy giao dịch này" };
            }
            if(purchased.Email == null)
            {
                return new ApiResult { Message = "Không có yêu cầu nào được thực hiện" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newNoti = new Notification
                {
                    TypeNoti = "Đã gửi Gmail",
                    Content = $"Giao dịch tài khoản mã số {purchased.Id}: đã gửi thông tin đăng ký tài khoản là {email}",
                    SenderID = _userService.UserId,
                    UserID = purchased.UserID,
                    IsRead = false,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                };
                purchased.Email = email;
                purchased.UpdatedDate = DateTime.Now;
                _dataContext.Notifications.Add(newNoti);
                _dataContext.PurchasedAccounts.Update(purchased);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Gửi email thất bại: {ex.Message}" };
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

        public async Task<ApiResult> OTPRequest(int id)
        {
            var purchased = await _dataContext.PurchasedAccounts.FirstOrDefaultAsync(x => x.Id == id);
            if (purchased == null || purchased.IsDelete == true)
            {
                return new ApiResult { Message = "Không tìm thấy giao dịch này" };
            }
            if (purchased.StatusBuyer == "Đã từ chối")
            {
                return new ApiResult { Message = "Bạn đã từ chối tài khoản này không thể yêu cầu gửi thông tin" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newNoti = new Notification
                {
                    TypeNoti = "Yêu cầu gửi OTP",
                    Content = $"Giao dịch tài khoản mã số {purchased.Id}: đã yêu cầu gửi OTP, để hoàn tất đổi mật khẩu",
                    SenderID = _userService.UserId,
                    UserID = purchased.SellerID,
                    IsRead = false,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                };
                _dataContext.Notifications.Add(newNoti);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Gửi email thất bại: {ex.Message}" };
            }
        }

        public async Task<ApiResult> OTPResponse(int id, string response)
        {
            var purchased = await _dataContext.PurchasedAccounts.FirstOrDefaultAsync(x => x.Id == id);
            if (purchased == null || purchased.IsDelete == true)
            {
                return new ApiResult { Message = "Không tìm thấy giao dịch này" };
            }
            if (purchased.StatusBuyer == "Đã từ chối")
            {
                return new ApiResult { Message = "Bạn đã từ chối tài khoản này không thể yêu cầu gửi thông tin" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newNoti = new Notification
                {
                    TypeNoti = "Đã gửi OTP",
                    Content = $"Giao dịch tài khoản mã số {purchased.Id}: đã gửi OTP là {response}",
                    SenderID = _userService.UserId,
                    UserID = purchased.UserID,
                    IsRead = false,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                };
                purchased.OTPEmail = response;
                purchased.UpdatedDate = DateTime.Now;
                _dataContext.Notifications.Add(newNoti);
                _dataContext.PurchasedAccounts.Update(purchased);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Gửi email thất bại: {ex.Message}" };
            }
        }
    }
}
