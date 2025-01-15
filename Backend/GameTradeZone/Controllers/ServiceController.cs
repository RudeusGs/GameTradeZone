using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.GameInfor;
using GameTradeZone.Service.Models.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : BaseController
    {
        public IServiceService _iServiceService;
        public ServiceController(IServiceService iServiceService)
        {
            _iServiceService = iServiceService;
        }
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll() 
        {
            try 
            {
                var result = await _iServiceService.GetAll();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
            
        }

        [HttpGet("Get-By-Id")]
        public async Task<IActionResult> GetById(int id) 
        {
            try
            {
                var result = await _iServiceService.GetById(id);
                return Response(result);
            }
            catch(Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddServiceModel model)
        {
            try
            {
                var result = await _iServiceService.Add(model);
                return Response(result);
            }
            catch( Exception e )
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Delete")]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var result = await _iServiceService.Delete(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Update")]
        public async Task<IActionResult> Update(UpdateServiceModel model)
        {
            try
            {
                var result = await _iServiceService.Update(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [Authorize]
        [HttpPost("RentService")]
        public async Task<IActionResult> RentService(RentedServiceModel model)
        {
            try
            {
                var result = await _iServiceService.RentedService(model);
                return Response(result);
            }
            catch(Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
