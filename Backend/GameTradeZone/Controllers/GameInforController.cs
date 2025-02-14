using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.GameInfor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameInforController : BaseController
    {
        public IGameInforService _iGameInforService;
        public GameInforController(IGameInforService iGameInforService)
        {
            _iGameInforService = iGameInforService;
        }
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _iGameInforService.GetAll();
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
                var result = await _iGameInforService.GetById(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddGameInforModel model)
        {
            try
            {
                var result = await _iGameInforService.Add(model);
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
                var result = await _iGameInforService.Delete(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Update")]
        public async Task<IActionResult> Update(UpdateGameInforModel model)
        {
            try
            {
                var result = await _iGameInforService.Update(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
