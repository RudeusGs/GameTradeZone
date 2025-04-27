using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.OngoingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnGoingServiceController : BaseController
    {
        private readonly IOnGoingServiceService _onGoingServiceService;

        public OnGoingServiceController(IOnGoingServiceService onGoingServiceService)
        {
            _onGoingServiceService = onGoingServiceService;
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _onGoingServiceService.GetAll();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-All-By-User-Id")]
        public async Task<IActionResult> GetAllByUserID(int id)
        {
            try
            {
                var result = await _onGoingServiceService.GetAllByUserID(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-ServiceName")]
        public async Task<IActionResult> GetServiceName(int id)
        {
            try
            {
                var result = await _onGoingServiceService.GetNameServiceByServiceId(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _onGoingServiceService.Delete(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Confirm-Service")]
        public async Task<IActionResult> ConfirmService(ConfirmServiceModel model)
        {
            try
            {
                var result = await _onGoingServiceService.ConfirmService(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-All-By-Service-Id")]
        public async Task<IActionResult> GetAllByServiceID(int id)
        {
            try
            {
                var result = await _onGoingServiceService.GetAllByServiceID(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        
        [Authorize]
        [HttpPost("Confirm-Extension")]
        public async Task<IActionResult> ConfirmExtension(int id, bool approve, TimeSpan? approvedTime = null)
        {
            try
            {
                var result = await _onGoingServiceService.ConfirmExtension(id, approve, approvedTime);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiResult { Message = e.Message });
            }
        }
        [HttpGet("Get-Remaining-Time")]
        public async Task<IActionResult> GetRemainingTime(int id)
        {
            try
            {
                var result = await _onGoingServiceService.GetRemainingTime(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}