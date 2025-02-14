using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Dispute;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Service.Services
{
    public class DisputeService : ServiceBase, IDisputeService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        private readonly FileUploadService _fileUploadService;
        public DisputeService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService, FileUploadService fileUploadService, IUserService userService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
            _fileUploadService = fileUploadService;
        }

        public async Task<ApiResult> Add(AddDisputeModel model)
        {
            var onGoing = await _dataContext.OnGoingServices.FirstOrDefaultAsync(x => x.Id == model.OnGoingServiceID);
            var hired = await _dataContext.HiredServices.FirstOrDefaultAsync(x => x.Id == model.HiredServiceID);
            var purChased = await _dataContext.PurchasedAccounts.FirstOrDefaultAsync(x => x.Id == model.PurchasedAccountID);

            if (onGoing == null && hired == null && purChased == null)
            {
                return new ApiResult { Message = "Thông tin không hợp lệ!" };
            }

            var existingDispute = await _dataContext.Disputes
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.IsDelete == false);

            if (existingDispute != null)
            {
                return new ApiResult { Message = "Thành phần này đã có!" };
            }

            using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newDispute = new Dispute
                {
                    OnGoingServiceID = model.OnGoingServiceID,
                    HiredServiceID = model.HiredServiceID,
                    PurchasedAccountID = model.PurchasedAccountID,
                    UserID = _userService.UserId,
                    Reason = model.Reason,
                    Status = "Đang chờ duyệt",
                    CreatedDate = DateTime.Now,
                    IsDelete = false,
                };

                if (model.OnGoingServiceID != null)
                {
                    var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.Id == onGoing.ServiceID);
                    newDispute.SellertID = service?.CreaterID;
                }
                else if (model.HiredServiceID != null)
                {
                    var service = await _dataContext.Services.FirstOrDefaultAsync(x => x.Id == hired.ServiceID);
                    newDispute.SellertID = service?.CreaterID;
                }
                else if (model.PurchasedAccountID != null)
                {
                    newDispute.SellertID = purChased?.SellerID;
                }

                _dataContext.Disputes.Add(newDispute);
                await _dataContext.SaveChangesAsync();

                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var uploadedImages = await fileUploadService.UploadFiles("dispute", newDispute.Id, model.Files);

                    if (uploadedImages.Any())
                    {
                        newDispute.Proof = string.Join(";", uploadedImages);
                        await _dataContext.SaveChangesAsync();
                    }
                }

                await tran.CommitAsync();
                return new ApiResult(newDispute);
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {ex.Message}" };
            }
        }


        public async Task<ApiResult> Delete(int id)
        {
            var dispute = await _dataContext.Disputes.FirstOrDefaultAsync(x => x.Id == id);
            if(dispute == null || dispute.IsDelete == true)
            {
                return new ApiResult { Message = "Thành phần này không tồn tại!" };
            }
            using var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                dispute.IsDelete = true;
                dispute.DeleteDate = DateTime.UtcNow;

                _dataContext.Disputes.Update(dispute);
                await _dataContext.SaveChangesAsync();

                await tran.CommitAsync();

                return new ApiResult();
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
            var dispute = await _dataContext.Disputes.Where(x => x.IsDelete == false).ToListAsync();
            return new(dispute);
        }

        public async Task<ApiResult> GetAllBySellerId(int id)
        {
            var dispute = await _dataContext.Disputes.Where(x => x.SellertID == id && x.IsDelete == false).ToListAsync();
            return new(dispute);
        }

        public async Task<ApiResult> GetAllByUserId(int id)
        {
            var dispute = await _dataContext.Disputes.Where(x => x.UserID == id && x.IsDelete == false).ToListAsync();
            return new(dispute);
        }

        public async Task<ApiResult> GetById(int id)
        {
            var dispute = await _dataContext.Disputes.Where(x => x.Id == id && x.IsDelete == false).FirstOrDefaultAsync();
            return new(dispute);
        }

        public async Task<ApiResult> Reply(int id, string rep)
        {
            var dispute = await _dataContext.Disputes.FirstOrDefaultAsync(x => x.Id == id);
            if(dispute == null)
            {
                return new ApiResult { Message = "Không tồn tại" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                dispute.Reply = rep;
                dispute.UpdatedDate = DateTime.Now;
                _dataContext.Disputes.Update(dispute);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult ();
            }
            catch(Exception e)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = $"Error: {e.Message}" };
            }
        }

        public async Task<ApiResult> Update(UpdateDisputeModel model)
        {
            var dispute = await _dataContext.Disputes.FirstOrDefaultAsync(x => x.Id == model.Id);
            if( dispute == null || dispute.IsDelete == true)
            {
                return new ApiResult { Message = "Không tồn tại!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                dispute.Reason = model.Reason ?? dispute.Reason;
                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var fileUploads = await fileUploadService.UploadFiles("service", dispute.Id, model.Files);

                    if (fileUploads.Any())
                    {
                        dispute.Proof = string.IsNullOrEmpty(dispute.Proof)
                            ? string.Join(";", fileUploads)
                            : $"{dispute.Proof};{string.Join(";", fileUploads)}";
                    }
                }

                dispute.UpdatedDate = DateTime.Now;
                _dataContext.Disputes.Update(dispute);
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
    }
}
