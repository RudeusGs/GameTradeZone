using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.Hubs;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.Auction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : BaseController
    {
        private readonly IAuctionService _auctionService;
        private readonly IHubContext<AuctionHub> _hubContext;

        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpGet("Get-All_Auction")]
        public async Task<IActionResult> GetAllAuction()
        {
            try
            {
                var result = await _auctionService.GetAllAuction();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-All_AuctionPrize")]
        public async Task<IActionResult> GetAllAuctionPrize()
        {
            try
            {
                var result = await _auctionService.GetAllAuctionPrize();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-All_Auction_Detail")]
        public async Task<IActionResult> GetAllAuctionDetail()
        {
            try
            {
                var result = await _auctionService.GetAllAuctionDetail();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Add_Auction")]
        public async Task<IActionResult> AddAuction(AddAuctionModel model)
        {
            try
            {
                var result = await _auctionService.AddAuction(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Add-Auction_Detail")]
        public async Task<IActionResult> AddAuctionDetail(AddAuctionDetailModel model)
        {
            try
            {
                var result = await _auctionService.AddAuctionDetail(model);
                if (result.IsSuccess)
                {
                    var auctionDetail = result.Data as AuctionDetail;
                    if (auctionDetail != null)
                    {
                        await _hubContext.Clients.Group("AuctionDetailGroup")
                            .SendAsync("AuctionDetailAdded", new
                            {
                                Id = auctionDetail.Id,
                                AuctionId = auctionDetail.AuctionId,
                                UserId = auctionDetail.UserId,
                                RaisePrice = auctionDetail.RaisePrice,
                                RaiseDateTime = auctionDetail.RaiseDateTime
                            });
                    }
                }
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-Auction-By-Id")]
        public async Task<IActionResult> GetAuctionById(int id)
        {
            try
            {
                var result = await _auctionService.GetAuctionById(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-Auction-Detail-By-Id")]
        public async Task<IActionResult> GetAuctionDetailById(int id)
        {
            try
            {
                var result = await _auctionService.GetAuctionDetailById(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpPost("Get-Auction-Prize_By_Id")]
        public async Task<IActionResult> GetAuctionPrizeById(int id)
        {
            try
            {
                var result = await _auctionService.GetAuctionPrizeById(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPut("Update-Auction_Detail")]
        public async Task<IActionResult> UpdateAuction([FromForm]UpdateAuctionModel model)
        {
            try
            {
                var result = await _auctionService.UpdateAuction(model);            
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpDelete("Delete-Auction")]
        public async Task<IActionResult> DeleteAuction(int id)
        {
            try
            {
                var result = await _auctionService.DeleteAuction(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpDelete("Delete-Auction-Detail")]
        public async Task<IActionResult> DeleteAuctionDetail(int id)
        {
            try
            {
                var result = await _auctionService.DeleteAuctionDetail(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpDelete("Delete-Auction-Prize")]
        public async Task<IActionResult> DeleteAuctionPrize(int id)
        {
            try
            {
                var result = await _auctionService.DeleteAuctionPrize(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
