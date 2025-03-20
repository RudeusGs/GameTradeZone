using GameTradeZone.Service.Models.AccountGame;
using GameTradeZone.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.Models.Auction;

namespace GameTradeZone.Service.Interfaces
{
    public interface IAuctionService
    {
        Task<ApiResult> GetAllAuction();
        Task<ApiResult> GetAllAuctionDetail();
        Task<ApiResult> GetAllAuctionPrize();


        Task<ApiResult> GetAuctionById(int id);
        Task<ApiResult> GetAuctionDetailById(int id);
        Task<ApiResult> GetAuctionPrizeById(int id);


        Task<ApiResult> AddAuction(AddAuctionModel model);
        Task<ApiResult> AddAuctionDetail(AddAuctionDetailModel model); // outbid (lệnh raise)
        //Task<ApiResult> AddAuctionPrize(AuctionPrize model);


        Task<ApiResult> UpdateAuction(UpdateAuctionModel model);
        Task<ApiResult> UpdateAuctionDetail(UpdateAuctionDetailModel model);
        Task<ApiResult> UpdateAuctionPrize(UpdateAuctionPrizeModel model);


        Task<ApiResult> DeleteAuction(int id);
        Task<ApiResult> DeleteAuctionDetail(int id);
        Task<ApiResult> DeleteAuctionPrize(int id);

        Task<ApiResult> EndAuction(int id);

    }
}
