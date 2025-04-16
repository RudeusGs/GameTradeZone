using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.AccountGame;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class AccountGameService : ServiceBase, IAccountGameService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        private readonly FileUploadService _fileUploadService;
        public AccountGameService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService,FileUploadService fileUploadService, IUserService userService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
            _fileUploadService = fileUploadService;
        }

        public async Task<ApiResult> Add(AddAccountGameModel model)
        {
            var accountGame = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.GameInforID == model.GameInforID && x.AccountName == model.AccountName);
            if(accountGame != null && accountGame.IsDelete == false)
            {
                return new ApiResult { Message = "Tài khoản game này đã tồn tại!" };
            }
            if (accountGame?.Status == "Đã bán")
            {
                return new ApiResult { Message = "Tài khoản này đã được bán" };
            }
            if(model.Price < model.PriceMin)
            {
                return new ApiResult { Message = "Số tiền đặt sai!"};
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newAccountGame = new AccountGame
                {
                    GameInforID = model.GameInforID,
                    AccountName = model.AccountName,
                    Password = model.Password,
                    Price = model.Price,
                    Status = "Đang bán",
                    PriceMin = model.PriceMin,
                    UserID = _userService.UserId,
                    CreatedDate = DateTime.Now,
                    IsDelete = false,
                };
                _dataContext.AccountGames.Add(newAccountGame);
                await _dataContext.SaveChangesAsync();

                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var uploadedImages = await fileUploadService.UploadFiles("gameaccount", newAccountGame.Id, model.Files);

                    if (uploadedImages.Any())
                    {
                        newAccountGame.Image = string.Join(";", uploadedImages);
                    }
                }

                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult(newAccountGame);
            }
            catch(Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {ex.Message}" };
            }
            
        }

        public async Task<ApiResult> Buy(BuyAccountGameModel model)
        {
            var accountGame = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (accountGame == null || accountGame.IsDelete == true)
            {
                return new ApiResult { Message = "Tài khoản này không tồn tại hoặc đã bị xóa!" };
            }
            if (accountGame.Status == "Đã bán")
            {
                return new ApiResult { Message = "Tài khoản này đã được bán!" };
            }
            var gameInfo = await _dataContext.GameInfors.FirstOrDefaultAsync(x => x.Id == accountGame.GameInforID);
            if (gameInfo == null)
            {
                return new ApiResult { Message = "Thông tin game không tồn tại!" };
            }
            var buyer = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == _userService.UserId);
            var seller = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == accountGame.UserID);
            if (buyer == null || seller == null)
            {
                return new ApiResult { Message = "Thông tin người mua không tồn tại!" };
            }
            if (buyer.Balance < accountGame.Price)
            {
                return new ApiResult { Message = "Số dư không đủ" };
            }
            if(buyer.Id == seller.Id )
            {
                return new ApiResult { Message = "Không thể tự mua tài khoản của chính mình" };
            }
            using (var tran = await _dataContext.Database.BeginTransactionAsync())
            {
                try
                {
                    buyer.Balance -= accountGame.Price;
                    _dataContext.Users.Update(buyer);

                    var newPurchasedAccount = new PurchasedAccount
                    {
                        UserID = _userService.UserId,
                        SellerID = accountGame.UserID,
                        GameName = gameInfo.GameName,
                        AccountName = accountGame.AccountName,
                        Password = accountGame.Password,
                        Price = accountGame.Price,
                        AccountGameId = accountGame.Id,
                        StatusBuyer = "Chưa xác nhận",
                        StatusSeller = "Đang chờ",
                        IsDelete = false,
                        CreatedDate = DateTime.Now,
                    };
                    
                    _dataContext.Add(newPurchasedAccount);
                    accountGame.Status = "Đã bán";
                    _dataContext.Update(accountGame);
                    await _dataContext.SaveChangesAsync();
                    await tran.CommitAsync();

                    return new ApiResult();
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    return new ApiResult { Message = $"Error: {ex.Message}" };
                }
            }
        }


        public async Task<ApiResult> Delete(int id)
        {
            var accountGame = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.Id == id);
            if(accountGame == null || accountGame.IsDelete == true)
            {
                return new ApiResult { Message = "Tài khoản này không tồn tại hoặc đã bị xóa" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                accountGame.IsDelete = true;
                accountGame.DeleteDate = DateTime.Now;
                _dataContext.Update(accountGame);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();

            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {ex.Message}" };
            }
        }


        public async Task<ApiResult> GetAll()
        {
            var accountGame = await _dataContext.AccountGames.Where(x => x.IsDelete == false).ToListAsync();
            return new(accountGame);
        }

        public async Task<ApiResult> GetAllByUserID(int UserId)
        {
            var accountGame = await _dataContext.AccountGames.Where(x => x.IsDelete == false && x.UserID == UserId).ToListAsync();
            return new(accountGame);
        }

        public async Task<ApiResult> GetById(int id)
        {
            var accountGame = await _dataContext.AccountGames.Where(x => x.IsDelete == false && x.Id == id).FirstOrDefaultAsync();
            return new(accountGame);
        }

        public async Task<ApiResult> GetInforUser(int id)
        {
            var a = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.Id  == id);
            var accountGame = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == a.UserID);
            return new(accountGame);
        }

        public async Task<ApiResult> Update(UpdateAccountGameModel model)
        {
            var accountGame = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (accountGame == null || accountGame.IsDelete == true)
            {
                return new ApiResult { Message = "Tài khoản này không tồn tại không thể cập nhật" };
            }

            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                accountGame.GameInforID = model.GameInforID ?? accountGame.GameInforID;
                accountGame.AccountName = model.AccountName ?? accountGame.AccountName;
                accountGame.Password = model.Password ?? accountGame.Password;
                accountGame.Price = model.Price != 0 ? model.Price : accountGame.Price;
                accountGame.PriceMin = model.PriceMin != 0 ? model.PriceMin : accountGame.Price;
                accountGame.UpdatedDate = DateTime.UtcNow;

                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var fileUploads = await fileUploadService.UploadFiles("gameaccount", accountGame.Id, model.Files);

                    if (fileUploads.Any())
                    {
                        accountGame.Image = string.IsNullOrEmpty(accountGame.Image)
                            ? string.Join(";", fileUploads)
                            : $"{accountGame.Image};{string.Join(";", fileUploads)}";
                    }
                }

                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {ex.Message}" };
            }
        }
          
        public async Task<ApiResult> GetAllUserDataStat()
        {
            var totalUsers = await _dataContext.Users.CountAsync();
            var newUsersByDay = await _dataContext.Users
                .GroupBy(u => u.CreatedDate.HasValue ? u.CreatedDate.Value.Date : DateTime.MinValue.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();
            var newUsersToday = newUsersByDay.FirstOrDefault(x => x.Date == DateTime.Now.Date);
            return new ApiResult(new { TotalUsers = totalUsers, NewUsersByDay = newUsersByDay ,NewUsersToday = newUsersToday });
        }

    }
}
