using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Service;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class ServiceService : ServiceBase, IServiceService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        private readonly FileUploadService _fileUploadService;
        public ServiceService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService, FileUploadService fileUploadService, IUserService userService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
            _fileUploadService = fileUploadService;
        }

        public async Task<ApiResult> Add(AddServiceModel model)
        {
            var gameInfor = await _dataContext.GameInfors.FirstOrDefaultAsync(x => x.Id == model.GameInforID);
            if(gameInfor == null || gameInfor.IsDelete == true)
            {
                return new ApiResult
                {
                    Message = "Game này không tồn tại không thể thêm dịch vụ"
                };
            }
            var service = await _dataContext.Services
            .FirstOrDefaultAsync(x => x.ServiceName == model.ServiceName && x.GameInforID == model.GameInforID && x.CreaterID == _userService.UserId);
            if(service != null && service.IsDelete == false)
            {
                return new ApiResult
                {
                    Message = "Bạn đã thêm dịch vụ này trước đó! Không thể thêm dịch vụ giống nhau."
                };
            }
            using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newService = new GameTradeZone.Domain.Entities.Service
                {
                    GameInforID = model.GameInforID,
                    ServiceName = model.ServiceName,
                    Decription = model.Decription,
                    ServicePrice = model.ServicePrice,
                    ServiceTime = model.ServiceTime,
                    CreaterID = _userService.UserId,
                    ServiceLevel = "Cấp 1",
                    RentedC = 0,
                    Feedback = null,
                    IsDelete = false,
                    CreatedDate = DateTime.UtcNow,
                };

                _dataContext.Services.Add(newService);
                await _dataContext.SaveChangesAsync();

                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var uploadedImages = await fileUploadService.UploadFiles("services", newService.Id, model.Files);

                    if (uploadedImages.Any())
                    {
                        newService.Image = string.Join(";", uploadedImages);
                    }
                }

                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();

                return new ApiResult { Message = "Thêm dịch vụ thành công!"};
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> Delete(int id)
        {
            var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.Id == id);

            if (service == null)
            {
                return new ApiResult
                {
                    Message = "Dịch vụ không tồn tại!"
                };
            }
            if(service.IsDelete == true)
            {
                return new ApiResult { Message = "Dịch vụ nãy đã được xóa! Không thể xóa nữa" };
            }

            using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                service.IsDelete = true;
                service.DeleteDate = DateTime.UtcNow;

                _dataContext.Services.Update(service);
                await _dataContext.SaveChangesAsync();

                await tran.CommitAsync();

                return new ApiResult
                {
                    Message = "Xóa thành công!"
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
            var services = await _dataContext.Services.Where(x => x.IsDelete == false).ToListAsync();
            return new(services);
        }

        public async Task<ApiResult> GetAllByUserId(int id)
        {
            var service = await _dataContext.Services.Where(x => x.CreaterID == id && x.IsDelete == false).ToListAsync();
            return new(service);
        }
        public async Task<ApiResult> GetById(int id)
        {
            var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == false);
            return new(service);
        }

        public async Task<ApiResult> GetGameById(int id)
        {
            var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.GameInforID == id && x.IsDelete == false);
            return new(service);
        }

        public async Task<ApiResult> RentedService(RentedServiceModel model)
        {
            var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.Id == model.Id);
            if(service == null)
            {
                return new ApiResult { Message = "Dịch vụ không tồn tại không thể thuê" };
            }
            if(service.IsDelete == true)
            {
                return new ApiResult { Message = "Dịch vụ đã bị xóa" };
            }

            var tran = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                var newOnGoing = new OnGoingService
                {
                    ServiceID = model.Id,
                    UserID = _userService.UserId,
                    Status = "Chờ xác nhận",
                    Reason = null,
                    FeedBack = null,
                    CreatedDate = DateTime.Now,
                };
                var newHired = new HiredService 
                { 
                    ServiceID = model.Id,
                    UserID = _userService.UserId,
                    Status = "Đang chờ xác nhận",
                    Reason = null,
                    CreatedDate= DateTime.Now,
                };
                _dataContext.OnGoingServices.Add(newOnGoing);
                _dataContext.HiredServices.Add(newHired);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();

                return new ApiResult { Message = "Thuê dịch vụ thành công! Vui lòng chờ xác nhận", };
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> Update(UpdateServiceModel model)
        {
            var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (service == null)
            {
                return new ApiResult { Message = "Không tìm thấy dịch vụ này." };
            }

            if (service.IsDelete == true)
            {
                return new ApiResult { Message = "Dịch vụ này đã bị xóa, không thể cập nhật." };
            }

            using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                service.ServiceName = model.ServiceName ?? service.ServiceName;
                service.Decription = model.Decription ?? service.Decription;
                service.ServicePrice = model.ServicePrice ?? service.ServicePrice;
                service.ServiceTime = model.ServiceTime ?? service.ServiceTime;
                service.UpdatedDate = DateTime.UtcNow;

                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var fileUploads = await fileUploadService.UploadFiles("service", service.Id, model.Files);

                    if (fileUploads.Any())
                    {
                        service.Image = string.IsNullOrEmpty(service.Image)
                            ? string.Join(";", fileUploads)
                            : $"{service.Image};{string.Join(";", fileUploads)}";
                    }
                }
                _dataContext.Services.Update(service);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();

                return new ApiResult { Message = "Cập nhật thành công!" };
            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }
    }
}
