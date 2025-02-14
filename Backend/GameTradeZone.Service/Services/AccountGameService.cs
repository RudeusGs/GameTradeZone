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

        public async Task<ApiResult> Accept(int id)
        {
            var accountGame = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.Id == id);
            if (accountGame == null || accountGame.IsDelete == true)
            {
                return new ApiResult { Message = "Tài khoản này không tồn tại!" };
            }
            if (accountGame.Status == "Đã bán")
            {
                return new ApiResult { Message = "Tài khoản này đã bán!" };
            }

            var bargain = await _dataContext.BargainAccounts.FirstOrDefaultAsync(x => x.AccountGameID == id);
            if (bargain == null)
            {
                return new ApiResult { Message = "Không tìm thấy thông tin thỏa thuận." };
            }

            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newPurchased = new PurchasedAccount
                {
                    GameName = accountGame.GameName,
                    AccountName = accountGame.AccountName,
                    Password = accountGame.Password,
                    SellerID = _userService.UserId,
                    UserID = bargain.UserID,
                    CreatedDate = DateTime.UtcNow,
                };

                _dataContext.PurchasedAccounts.Add(newPurchased);
                accountGame.Status = "Đã bán";
                _dataContext.AccountGames.Update(accountGame);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();

                return new ApiResult(newPurchased);
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {ex.Message}" };
            }
        }


        public async Task<ApiResult> Add(AddAccountGameModel model)
        {
            var accountGame = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.GameName == model.GameName && x.AccountName == model.AccountName);
            if(accountGame != null && accountGame.IsDelete == false)
            {
                return new ApiResult { Message = "Tài khoản game này đã tồn tại!" };
            }
            if (accountGame?.Status == "Đã bán")
            {
                return new ApiResult { Message = "Tài khoản này đã được bán" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newAccountGame = new AccountGame
                {
                    GameInforID = model.GameInforID,
                    GameName = model.GameName,
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

        public async Task<ApiResult> BargainPrice(BargainAccountGameModel model)
        {
            var accountGame = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.Id == model.Id);
            if(accountGame == null || accountGame.IsDelete == true) {
                return new ApiResult { Message = "Tài khoản này không tồn tại" };
            }   
            if(accountGame.Status == "Đã bán")
            {
                return new ApiResult { Message = "Tài khoản này đã được bán không thể đặt giá" };
            }
            if(accountGame.PriceMin > model.BargainAmount)
            {
                return new ApiResult { Message = "Không thể trả giá thấp hơn giá nhỏ nhất" };
            }
            if(model.BargainAmount % 1000 == 0)
            {
                return new ApiResult { Message = "Số tiền đặt phải chia hết cho 1000" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newBargain = new BargainAccountGame
                {
                    AccountGameID = model.Id,
                    UserID = _userService.UserId,
                    BargainPrice = model.BargainAmount,
                    CreatedDate = DateTime.UtcNow,
                };
                var newAccountGame = new AccountGame
                {
                    PriceMin = model.BargainAmount,
                };
                _dataContext.BargainAccounts.Add(newBargain);
                _dataContext.AccountGames.Update(newAccountGame);
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

        public async Task<ApiResult> Buy(BuyAccountGameModel model)
        {
            var accountGame = await _dataContext.AccountGames.FirstOrDefaultAsync(x => x.Id == model.Id);
            if(accountGame == null || accountGame.IsDelete == true) 
            {
                return new ApiResult { Message = "Tài khoản này không tồn tại hoặc đã bị xóa!" };
            }
            if(accountGame.Status == "Đã bán")
            {
                return new ApiResult { Message = "Tài khoản này đã được bán!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newPurchasedAccount = new PurchasedAccount
                {
                    UserID = _userService.UserId,
                    SellerID = accountGame.UserID,
                    GameName = accountGame.GameName,
                    AccountName = accountGame.AccountName,
                    Password = accountGame.Password,
                    StatusBuyer = null,
                    StatusSeller = "Đang chờ",
                    CreatedDate = DateTime.Now,
                };
                _dataContext.Add(newPurchasedAccount);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();
            }
            catch(Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {ex.Message}" };
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

        public async Task<ApiResult> GetInforUser()
        {
            var accountGame = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == _userService.UserId);
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
                accountGame.GameName = model.GameName ?? accountGame.GameName;
                accountGame.AccountName = model.AccountName ?? accountGame.AccountName;
                accountGame.Password = model.Password ?? accountGame.Password;
                accountGame.Price = model.Price != 0 ? model.Price : accountGame.Price;
                accountGame.UpdatedDate = DateTime.UtcNow;

                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var fileUploads = await fileUploadService.UploadFiles("accountgame", accountGame.Id, model.Files);

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

    }
}
