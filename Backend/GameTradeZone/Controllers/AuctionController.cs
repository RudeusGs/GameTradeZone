using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Hubs;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.Auction;
using GameTradeZone.Service.Services;
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
        private readonly IUserService _userService; 

        public AuctionController(
            IHubContext<AuctionHub> hubContext,
            IAuctionService auctionService,
            IUserService userService)
        {
            _hubContext = hubContext;
            _auctionService = auctionService;
            _userService = userService;
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

        [HttpGet("Get-All_Auction-Detail")]
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
        [HttpPost("Add-Auction-Detail")]
        public async Task<IActionResult> AddAuctionDetail([FromBody]AddAuctionDetailModel model)
        {
            try
            {
                // Lấy UserId từ token
                int userId = _userService.UserId;
                if (userId == 0)
                {
                    throw new Exception("User ID not found");
                }

                var result = await _auctionService.AddAuctionDetail(model);
                if (result.IsSuccess)
                {
                    // Lấy thông tin trực tiếp từ model
                    int auctionId = model.AuctionId;
                    if (!int.TryParse(model.RaisePrice, out int newPrice))
                    {
                        return Response("Giá đặt không hợp lệ", 400);
                    }

                    // Gửi thông báo cập nhật giá qua SignalR
                    await _hubContext.Clients.Group($"Auction_{auctionId}")
                        .SendAsync("PriceUpdate", auctionId, newPrice, userId);
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

        [HttpGet("Get-Auction-Prize-By-Id")]
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
        [HttpPut("Update-Auction-Detail")]
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

        [Authorize]
        [HttpPost("End-Auction")]
        public async Task<IActionResult> EndAuction([FromForm]int id, int WinnerId, DateTime EndDatetime)
        {
            try
            {
                var result = await _auctionService.EndAuction(id, WinnerId, EndDatetime);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [Authorize]
        [HttpPut("Update-Auction-Prize")]
        public async Task<IActionResult> UpdateAuctionPrize([FromForm] UpdateAuctionPrizeModel model)
        {
            try
            {
                var result = await _auctionService.UpdateAuctionPrize(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
