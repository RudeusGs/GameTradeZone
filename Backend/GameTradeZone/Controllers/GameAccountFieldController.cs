using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.GameAccountField;
using GameTradeZone.Service.Models.GameField;
using GameTradeZone.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameAccountFieldController : BaseController
    {
        private readonly IGameAccountFieldService _gameAccountFieldService;
        public GameAccountFieldController(IGameAccountFieldService gameAccountFieldService)
        {
            _gameAccountFieldService = gameAccountFieldService;
        }
        [Authorize]
        [HttpPost("Add-Field-For-Game")]
        public async Task<IActionResult> Add(AddGameAccountFieldModel model)
        {
            try
            {
                var result = await _gameAccountFieldService.Add(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [HttpGet("Get-By-Id-For-Game")]
        public async Task<IActionResult> GetById(int id, int id2)
        {
            try
            {
                var result = await _gameAccountFieldService.GetByID(id, id2);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
