using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailLogController : BaseController
    {
        private readonly IEmailLogService _emailLogService;

        public EmailLogController(IEmailLogService emailLogService)
        {
            _emailLogService = emailLogService;
        }
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _emailLogService.GetAll();
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
