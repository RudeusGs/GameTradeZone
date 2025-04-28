using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.HiredService;
using GameTradeZone.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiredServiceController : BaseController
    {
        private readonly IHiredServiceService _hiredServiceService;

        public HiredServiceController(IHiredServiceService hiredServiceService)
        {
            _hiredServiceService = hiredServiceService;
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _hiredServiceService.GetAll();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [HttpGet("Get-All-By-User-Id")]
        public async Task<IActionResult> GetAllByUserId(int id)
        {
            try
            {
                var result = await _hiredServiceService.GetAllByUserId(id);
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
                var result = await _hiredServiceService.Delete(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpGet("Get-All-By-Service-Id")]
        public async Task<IActionResult> GetAllByServiceId(int id)
        {
            try
            {
                var result = await _hiredServiceService.GetAllByServiceID(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [Authorize]
        [HttpPost("Done-Service")]
        public async Task<IActionResult> DoneService(int id)
        {
            try
            {
                var result = await _hiredServiceService.DoneService(id);
                return Response(result);
            }
            catch(Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Confirm-Service")]
        public async Task<IActionResult> ConfirmService(AcceptServiceModel model)
        {
            try
            {
                var result = await _hiredServiceService.ConfirmService(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Extend-Time")]
        public async Task<IActionResult> ExtendTime(int id, TimeSpan extensionTime)
        {
            try
            {
                var result = await _hiredServiceService.ExtendTime(id, extensionTime);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiResult { Message = e.Message });
            }
        }
    }
}
