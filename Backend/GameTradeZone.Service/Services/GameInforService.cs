using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.GameInfor;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class GameInforService : ServiceBase, IGameInforService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        private readonly FileUploadService _fileUploadService;
        public GameInforService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService, FileUploadService fileUploadService, IUserService userService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
            _fileUploadService = fileUploadService;
        }

        public async Task<ApiResult> Add(AddGameInforModel model)
        {
            var gameInfor = await _dataContext.GameInfors.FirstOrDefaultAsync(x => x.GameName == model.GameName);
            if(gameInfor != null && gameInfor.IsDelete == false) 
            {
                return new ApiResult { Message = "Game này đã tồn tại vui lòng thêm game khác" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newGameInfor = new GameInfor
                {
                    GameName = model.GameName,
                    Genre = model.Genre,
                    IsDelete = false,
                    CreatedDate = DateTime.Now,
                };

                _dataContext.GameInfors.Add(newGameInfor);
                await _dataContext.SaveChangesAsync();

                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var uploadedImages = await fileUploadService.UploadFiles("gameinfor", newGameInfor.Id, model.Files);

                    if (uploadedImages.Any())
                    {
                        newGameInfor.Image = string.Join(";", uploadedImages);
                    }
                }

                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();

                return new ApiResult { Message = "Thêm thành công!"};
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResult> Delete(int id)
        {
            var gameInfor = await _dataContext.GameInfors.FirstOrDefaultAsync(x => x.Id == id);

            if (gameInfor == null || gameInfor.IsDelete == true)
            {
                return new ApiResult
                {
                    Message = "Game không tồn tại!"
                };
            }

            using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                gameInfor.IsDelete = true;
                gameInfor.DeleteDate = DateTime.UtcNow;

                _dataContext.GameInfors.Update(gameInfor);
                await _dataContext.SaveChangesAsync();

                await tran.CommitAsync();

                return new ApiResult
                {
                    Message = "Game này đã được xóa!"
                };
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult
                {
                    Message = $"Error: {e.Message}"
                };
            }
        }

        public async Task<ApiResult> GetAll()
        {
            var gameInfor = await _dataContext.GameInfors.Where(x => x.IsDelete == false).ToListAsync();
            return new(gameInfor);
        }

        public async Task<ApiResult> GetByGameFieldID(int id)
        {
            var gameInfor = await _dataContext.GameFields.Where(x => x.GameInforID == id && x.IsDelete == false).ToListAsync();
            return new(gameInfor);
        }

        public async Task<ApiResult> GetById(int id)
        {
            var gameInfor = await _dataContext.GameInfors.FirstOrDefaultAsync(x =>x.Id == id && x.IsDelete == false);
            return new(gameInfor);
        }

        public async Task<ApiResult> Update(UpdateGameInforModel model)
        {
            var gameInfor = await _dataContext.GameInfors.FirstOrDefaultAsync(x => x.Id == model.Id);
            if(gameInfor == null)
            {
                return new ApiResult { Message = "Game này không tồn tại không thể cập nhật" };
            }
            if(gameInfor.IsDelete == true)
            {
                return new ApiResult { Message = "Game này đã xóa không thể cập nhật" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                gameInfor.GameName = model.GameName ?? gameInfor.GameName;
                gameInfor.Genre = model.Genre ?? gameInfor.Genre;
                gameInfor.UpdatedDate = DateTime.Now;         
                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var fileUploads = await fileUploadService.UploadFiles("gameinfor", gameInfor.Id, model.Files);

                    if (fileUploads.Any())
                    {
                        gameInfor.Image = string.IsNullOrEmpty(gameInfor.Image)
                            ? string.Join(";", fileUploads)
                            : $"{gameInfor.Image};{string.Join(";", fileUploads)}";
                    }
                }
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = "Cập nhật thành công!" };

            }
            catch(Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult
                {
                    Message = $"Error: {e.Message}"
                };
            }
        }
    }
}
