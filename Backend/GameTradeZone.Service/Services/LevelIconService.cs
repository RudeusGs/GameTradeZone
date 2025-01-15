using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.GameInfor;
using GameTradeZone.Service.Models.LevelIcon;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class LevelIconService : ServiceBase, ILevelIconService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        private readonly FileUploadService _fileUploadService;
        public LevelIconService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService, FileUploadService fileUploadService, IUserService userService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
            _fileUploadService = fileUploadService;
        }

        public async Task<ApiResult> Add(AddLevelIconModel model)
        {
            var icon = await _dataContext.LevelIcons.FirstOrDefaultAsync(x => x.IconName == model.IconName);
            if(icon != null && icon.IsDelete == false)
            {
                return new ApiResult { Message = "Icon này đã có" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newIcon = new LevelIcon
                {
                    IconName = model.IconName,
                    CreatedDate = DateTime.Now,
                    IsDelete = false,
                };
                _dataContext.LevelIcons.Add(newIcon);
                await _dataContext.SaveChangesAsync();
                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var uploadedImages = await fileUploadService.UploadFiles("levelicon", newIcon.Id, model.Files);

                    if (uploadedImages.Any())
                    {
                        newIcon.IconImage = string.Join(";", uploadedImages);
                    }
                }
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Message = "Thêm thành công!" };
            }
            catch(Exception e)
            {
                await tran.CommitAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> Delete(int id)
        {
            var icon = await _dataContext.LevelIcons.FirstOrDefaultAsync(x => x.Id == id);
            if(icon == null || icon.IsDelete == true) 
            {
                return new ApiResult { Message = "Icon này không tồn tại" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                icon.IsDelete = true;
                icon.DeleteDate = DateTime.Now;

                _dataContext.LevelIcons.Update(icon);
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
            var icon = await _dataContext.LevelIcons.Where(x => x.IsDelete == false).ToListAsync();
            return new(icon);
        }

        public async Task<ApiResult> GetById(int id)
        {
            var icon = await _dataContext.LevelIcons.Where(x => x.Id == id && x.IsDelete == false).FirstOrDefaultAsync(); 
            return new(icon);
        }

        public async Task<ApiResult> Update(UpdateLevelIconModel model)
        {
            var icon = await _dataContext.LevelIcons.FirstOrDefaultAsync(x => x.Id == model.Id);
            if(icon == null || icon.IsDelete == true)
            {
                return new ApiResult { Message = "Icon này không tồn tại!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                icon.IconName = model.IconName ?? icon.IconName;
                icon.UpdatedDate = DateTime.Now;
                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var fileUploads = await fileUploadService.UploadFiles("levelicon", icon.Id, model.Files);

                    if (fileUploads.Any())
                    {
                        icon.IconImage = string.IsNullOrEmpty(icon.IconImage)
                            ? string.Join(";", fileUploads)
                            : $"{icon.IconImage};{string.Join(";", fileUploads)}";
                    }
                }
                _dataContext.LevelIcons.Update(icon);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();

                return new ApiResult { Message = "Cập nhật thành công!" };
            }
            catch(Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }
    }
}
