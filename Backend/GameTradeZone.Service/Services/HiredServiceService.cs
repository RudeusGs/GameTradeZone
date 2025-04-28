using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Common.Services;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.HiredService;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class HiredServiceService : ServiceBase, IHiredServiceService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        private readonly FileUploadService _fileUploadService;
        public HiredServiceService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService, FileUploadService fileUploadService, IUserService userService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
            _fileUploadService = fileUploadService;
        }
        public async Task<ApiResult> ConfirmService(AcceptServiceModel model)
        {
            var hiredService = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (hiredService == null || hiredService.IsDelete == true)
            {
                return new ApiResult { Message = "Không tìm thấy thành phần này!" };
            }
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == hiredService.UserID);
            if (user == null)
            {
                return new ApiResult { Message = "Người dùng không tồn tại!" };
            }
            var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.Id == hiredService.ServiceID);
            if (service == null)
            {
                return new ApiResult { Message = "Dịch vụ không tồn tại" };
            }
            var ongoingService = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.Id == hiredService.OnGoingServiceId);
            if (ongoingService == null)
            {
                return new ApiResult { Message = "Không tìm thấy thành phần này!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                if (model.Status == "Từ chối")
                {
                    user.Balance += service.ServicePrice;
                    hiredService.Status = "Đã từ chối";
                    ongoingService.Status = "Từ chối nhận";
                    ongoingService.Reason = model.Reason;
                    _dataContext.Users.Update(user);
                    _dataContext.HiredServices.Update(hiredService);
                    _dataContext.OnGoingServices.Update(ongoingService);
                    await _dataContext.SaveChangesAsync();
                    await tran.CommitAsync();
                    return new ApiResult();
                }
                else if (model.Status == "Đồng ý")
                {
                    hiredService.Status = "Trạng thái chờ";
                    hiredService.StartTime = DateTime.Now;
                    hiredService.EndTime = hiredService.StartTime + service.ServiceTime;
                    ongoingService.Status = "Đã duyệt, vui lòng chờ";
                    ongoingService.EndTime = hiredService.StartTime + service.ServiceTime;
                    _dataContext.HiredServices.Update(hiredService);
                    _dataContext.OnGoingServices.Update(ongoingService);
                    await _dataContext.SaveChangesAsync();
                    await tran.CommitAsync();
                    return new ApiResult();
                }
                else
                {
                    return new ApiResult { Message = "Trạng thái không đúng!" };
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
            var hiredService = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.Id == id);
            if (hiredService == null || hiredService.IsDelete == true)
            {
                return new ApiResult { Message = "Không tìm thấy thành phần này!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                hiredService.IsDelete = true;
                hiredService.DeleteDate = DateTime.Now;

                _dataContext.HiredServices.Update(hiredService);
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

        public async Task<ApiResult> DoneService(int id)
        {
            var hiredService = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.Id == id);
            if (hiredService == null)
            {
                return new ApiResult { Message = "Không tìm thấy dịch vụ này" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            var ongoingService = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.Id == hiredService.OnGoingServiceId);
            if (ongoingService == null)
            {
                return new ApiResult { Message = "Không tìm thấy dich vụ này" };
            }
            try
            {
                hiredService.Status = "Thành công";
                ongoingService.Status = "Dịch vụ đã xong, vui lòng kiểm tra trước khi xác nhận";
                hiredService.UpdatedDate = DateTime.Now;
                ongoingService.UpdatedDate = DateTime.Now;
                _dataContext.HiredServices.Update(hiredService);
                _dataContext.OnGoingServices.Update(ongoingService);
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

        public async Task<ApiResult> ExtendTime(int Id, TimeSpan extensionTime)
        {
            var onGoingService = await _dataContext.HiredServices
                .FirstOrDefaultAsync(x => x.Id == Id && x.IsDelete == false);
            if (onGoingService == null)
            {
                return new ApiResult { Message = "Không tìm thấy dịch vụ này" };
            }
            if (onGoingService.Status != "Trạng thái chờ")
            {
                return new ApiResult { Message = "Dịch vụ không ở trạng thái có thể gia hạn" };
            }
            var hiredService = await _dataContext.OnGoingServices
                .FirstOrDefaultAsync(x => x. == Id && x.IsDelete == false);
            if (hiredService == null)
            {
                return new ApiResult { Message = "Không tìm thấy dịch vụ thuê tương ứng" };
            }
            if (hiredService.ExtensionRequested)
            {
                return new ApiResult { Message = "Đã có yêu cầu gia hạn đang chờ xử lý" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                hiredService.ExtensionRequested = true;
                hiredService.RequestedExtensionTime = extensionTime;
                _dataContext.HiredServices.Update(hiredService);
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
            var hiredService = await _dataContext.HiredServices.Where(x => x.IsDelete == false).ToListAsync();
            return new(hiredService);
        }

        public async Task<ApiResult> GetAllByServiceID(int id)
        {
            var hiredService = await _dataContext.HiredServices.Where(x => x.ServiceID == id && x.IsDelete == false).ToListAsync();
            return new(hiredService);
        }

        public async Task<ApiResult> GetAllByUserId(int id)
        {
            var hiredService = await _dataContext.HiredServices.Where(x => x.UserID == id && x.IsDelete == false).ToListAsync();
            return new(hiredService);
        }

        public async Task<ApiResult> SendProof(ProofDoneService model)
        {
            var hiredService = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (hiredService == null || hiredService.IsDelete == true)
            {
                return new ApiResult { Message = "Không tìm thấy thành phần này" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                hiredService.Decriptions = model.Decription;
                _dataContext.HiredServices.Update(hiredService);
                await _dataContext.SaveChangesAsync();
                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var uploadedImages = await fileUploadService.UploadFiles("proofservice", hiredService.Id, model.Files);

                    if (uploadedImages.Any())
                    {
                        hiredService.Image = string.Join(";", uploadedImages);
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