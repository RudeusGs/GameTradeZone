using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.Auction;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Services
{

    public class AuctionService : ServiceBase, IAuctionService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;
        private readonly FileUploadService _fileUploadService;
        private readonly CloudinaryService _cloudinaryService;
        public AuctionService(DataContext dataContext, IFtpDirectoryService ftpDirectoryService, FileUploadService fileUploadService, IUserService userService, CloudinaryService cloudinaryService) : base(dataContext, userService)
        {
            _ftpDirectoryService = ftpDirectoryService;
            _fileUploadService = fileUploadService;
            _cloudinaryService = cloudinaryService;

        }

        public async Task<ApiResult> AddAuction(AddAuctionModel model)
        {
            var auctions = await _dataContext.Auctions.FirstOrDefaultAsync(x => x.AuctionName == model.AuctionName);
            var auctionPrizes = await _dataContext.AuctionPrizes.FirstOrDefaultAsync(x => x.PrizeName == model.PrizeName);
            if (auctions != null)
            {
                return new ApiResult { Message = "Tên của phiên đấu giá đã tồn tại!" };
            }
            if (auctionPrizes != null)
            {
                return new ApiResult { Message = "Tên của phần thưởng đã tồn tại!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var newAuction = new Auction
                {
                    UserId = _userService.UserId,
                    AuctionName = model.AuctionName,
                    StartDateTime = model.StartDateTime,
                    StartPrice = model.StartPrice,
                    CurrentPrice = model.StartPrice,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                };
                await _dataContext.Auctions.AddAsync(newAuction);
                await _dataContext.SaveChangesAsync();

                var newAuctionPrize = new AuctionPrize
                {
                    Id = newAuction.Id,
                    AuctionId = newAuction.Id,
                    PrizeName = model.PrizeName,
                    Description = model.PrizeDescription,
                    PrizeInfo = model.PrizeInfo,
                    Status = false,
                    CreatedDate = _now,
                    UpdatedDate = _now,
                };
                await _dataContext.AuctionPrizes.AddAsync(newAuctionPrize);
                await _dataContext.SaveChangesAsync();

                newAuction.AuctionPrizeId = newAuctionPrize.Id;
                _dataContext.Auctions.Update(newAuction);
                await _dataContext.SaveChangesAsync();

                if (model.PrizeImage != null && model.PrizeImage.Any())
                {
                    var uploadedImages = await _cloudinaryService.UploadMutilImage(model.PrizeImage);
                    if (uploadedImages.Any())
                    {
                        newAuctionPrize.Image = string.Join(";", uploadedImages);
                        _dataContext.AuctionPrizes.Update(newAuctionPrize);
                        await _dataContext.SaveChangesAsync();
                    }
                }

                await tran.CommitAsync();
                return new ApiResult(new
                {
                    Auction = newAuction,
                    AuctionPrize = newAuctionPrize
                });
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = ex.Message };
            }
        }

        public async Task<ApiResult> AddAuctionDetail(AddAuctionDetailModel model)
        {
            var auction = await _dataContext.Auctions.FirstOrDefaultAsync(x => x.Id == model.AuctionId);
            //var addAuctionDetail = await _dataContext.AuctionDetails.FirstOrDefaultAsync(x => x.AuctionId == auction.Id);
            //if (addAuctionDetail != null)
            //{
            //    return new ApiResult { Message = "Lệnh raise bị lỗi!" };
            //} 
            // kiểm tra tồn tại của lệnh raise có 2 trường hợp
            // 1: chưa có lệnh raise nào -> làm như trên sẽ lỗi vì không có lệnh raise nào tương ứng với phiên đấu giá đó
            // 2: nếu tồn tại một lệnh raise của phiên đó rồi -> có thể dùng cách trên
            if (auction == null)
            {
                return new ApiResult { Message = "Phiên đấu giá Không tồn tại!" };
            }
            if (auction.IsApproved == false
                || auction.EndStatus == true
                || !auction.StartDateTime.HasValue
                || _now < auction.StartDateTime.Value)
            {
                return new ApiResult { Message = "Phiên đấu giá này đang xem xét hoặc đã kết thúc" };
            }
            if (int.TryParse(model.RaisePrice, out int raisePrise)
                && int.TryParse(auction.CurrentPrice, out int currentPrice))
            {
                if (raisePrise <= currentPrice)
                {
                    return new ApiResult { Message = "Giá của bạn không đúng!" };
                }
            }
            else
            {
                return new ApiResult { Message = "Giá không hợp lệ!" };
            }    


            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var bid = new AuctionDetail
                {
                    AuctionId = auction.Id,
                    UserId = _userService.UserId,
                    RaisePrice = model.RaisePrice,
                    RaiseDateTime = _now,
                };
                await _dataContext.AuctionDetails.AddAsync(bid);
                await _dataContext.SaveChangesAsync();

                auction.CurrentPrice = model.RaisePrice;
                await _dataContext.SaveChangesAsync();

                await tran.CommitAsync();
                return new ApiResult { Data = bid };

            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = "Lỗi: " + ex.Message };    
            }
        }

        public async Task<ApiResult> DeleteAuction(int id)
        {
            var auction = _dataContext.Auctions.FirstOrDefault(x => x.Id == id);
            if (auction == null || auction.DeleteDate != null)
            {
                return new ApiResult { Message = "Phiên đấu giá không tồn tại hoặc đã bị xóa!" };
            }
            var tran = _dataContext.Database.BeginTransaction();
            try
            {
                auction.DeleteDate = _now;
                _dataContext.Auctions.Update(auction);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = ex.Message };
            }
        }

        public async Task<ApiResult> DeleteAuctionDetail(int id)
        {
            var auctionDetail = _dataContext.AuctionDetails.FirstOrDefault(x => x.Id == id);
            if (auctionDetail == null || auctionDetail.DeleteDate != null)
            {
                return new ApiResult { Message = "Phiên đấu giá không tồn tại hoặc đã bị xóa!" };
            }
            var tran = _dataContext.Database.BeginTransaction();
            try
            {
                auctionDetail.DeleteDate = _now;
                _dataContext.AuctionDetails.Update(auctionDetail);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = ex.Message };
            }
        }

        public async Task<ApiResult> DeleteAuctionPrize(int id)
        {
            var auctionPrize = _dataContext.AuctionPrizes.FirstOrDefault(x => x.Id == id);
            if (auctionPrize == null || auctionPrize.DeleteDate != null)
            {
                return new ApiResult { Message = "Phiên đấu giá không tồn tại hoặc đã bị xóa!" };
            }
            var tran = _dataContext.Database.BeginTransaction();
            try
            {
                auctionPrize.DeleteDate = _now;
                _dataContext.AuctionPrizes.Update(auctionPrize);
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = ex.Message };
            }
        }

        public Task<ApiResult> EndAuction(int id)
        {

            throw new NotImplementedException();
        }

        public async Task<ApiResult> GetAllAuction()
        {
            var Auctions = await _dataContext.Auctions.Where(x => x.DeleteDate == null).ToListAsync();
            return new(Auctions);
        }

        public async Task<ApiResult> GetAllAuctionDetail()
        {
            var AuctionDetails = await _dataContext.AuctionDetails.Where(x => x.DeleteDate == null).ToListAsync();
            return new(AuctionDetails);
        }

        public async Task<ApiResult> GetAllAuctionPrize()
        {
            var AuctionPrize = await _dataContext.AuctionPrizes.Where(x => x.DeleteDate == null).ToListAsync();
            return new(AuctionPrize);
        }

        public async Task<ApiResult> GetAuctionById(int id)
        {
            var auction = await _dataContext.Auctions.FirstOrDefaultAsync(x => x.Id == id);
            return new(auction);
        }

        public async Task<ApiResult> GetAuctionDetailById(int id)
        {
            var auctionDetail = await _dataContext.AuctionDetails.FirstOrDefaultAsync(x => x.Id == id);
            return new(auctionDetail);
        }

        public async Task<ApiResult> GetAuctionPrizeById(int id)
        {
            var auctionPrize = await _dataContext.AuctionPrizes.FirstOrDefaultAsync(x => x.Id == id);
            return new(auctionPrize);
        }

        public async Task<ApiResult> UpdateAuction(UpdateAuctionModel model)
        {
            var auction = await _dataContext.Auctions.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (auction == null)
            {
                return new ApiResult { Message = "Không tìm thấy phiên đấu giá này!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                if (model.UserId.HasValue)
                    auction.UserId = model.UserId.Value;

                if (!string.IsNullOrEmpty(model.AuctionName))
                    auction.AuctionName = model.AuctionName;

                if (model.StartDateTime.HasValue)
                    auction.StartDateTime = model.StartDateTime.Value;

                if (!string.IsNullOrEmpty(model.StartPrice))
                    auction.StartPrice = model.StartPrice;

                if (!string.IsNullOrEmpty(model.CurrentPrice))
                    auction.CurrentPrice = model.CurrentPrice;

                if (!string.IsNullOrEmpty(model.TimeToEnd))
                    auction.TimeToEnd = model.TimeToEnd;

                if (model.EndStatus.HasValue)
                    auction.EndStatus = model.EndStatus.Value;

                if (model.IsApproved.HasValue)
                    auction.IsApproved = model.IsApproved.Value;

                if (model.WinnerId.HasValue)
                    auction.WinnerId = model.WinnerId.Value;
                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Data = auction };
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = ex.Message };
            }
        }

        public async Task<ApiResult> UpdateAuctionDetail(UpdateAuctionDetailModel model)
        {
            var auctionDetail = await _dataContext.AuctionDetails.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (auctionDetail == null)
            {
                return new ApiResult { Message = "Không tìm thấy phiên đấu giá này!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                    if (model.UserId.HasValue)
                        auctionDetail.UserId = model.UserId;

                    if (model.AuctionId.HasValue)
                        auctionDetail.AuctionId = model.AuctionId;

                    if (model.UserId.HasValue)
                        auctionDetail.UserId = model.UserId;

                    if (!string.IsNullOrEmpty(model.RaisePrice))
                        auctionDetail.RaisePrice = model.RaisePrice;

                    if (model.RaiseDateTime.HasValue)
                        auctionDetail.RaiseDateTime = model.RaiseDateTime;

                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Data = auctionDetail };
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = ex.Message };
            }
        }

        public async Task<ApiResult> UpdateAuctionPrize(UpdateAuctionPrizeModel model)
        {
            var auctionPrize = await _dataContext.AuctionPrizes.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (auctionPrize == null)
            {
                return new ApiResult { Message = "Không tìm thấy phiên đấu giá này!" };
            }
            var tran = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                if (model.AuctionId.HasValue)
                    auctionPrize.AuctionId = model.AuctionId.Value;

                if (!string.IsNullOrEmpty(model.PrizeName))
                    auctionPrize.PrizeName = model.PrizeName;

                if (!string.IsNullOrEmpty(model.Description))
                    auctionPrize.Description = model.Description;

                if (!string.IsNullOrEmpty(model.PrizeInfo))
                    auctionPrize.PrizeInfo = model.PrizeInfo;

                if (model.Status.HasValue)
                    auctionPrize.Status = model.Status;

                if (model.Files != null && model.Files.Any())
                {
                    var fileUploadService = new FileUploadService(_ftpDirectoryService);
                    var fileUploads = await fileUploadService.UploadFiles("auctionprizes", auctionPrize.Id, model.Files);

                    if (fileUploads.Any())
                    {
                        auctionPrize.Image = string.IsNullOrEmpty(auctionPrize.Image)
                            ? string.Join(";", fileUploads)
                            : $"{auctionPrize.Image};{string.Join(";", fileUploads)}";
                    }
                }


                await _dataContext.SaveChangesAsync();
                await tran.CommitAsync();
                return new ApiResult { Data = auctionPrize };
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return new ApiResult { Message = ex.Message };
            }
            throw new NotImplementedException();
        }
    }
}
