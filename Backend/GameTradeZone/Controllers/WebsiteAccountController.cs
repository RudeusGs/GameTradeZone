using GameTradeZone.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteAccountController : BaseController
    {
        private readonly IWebsiteAccountService _websiteAccountService;
        public WebsiteAccountController(IWebsiteAccountService websiteAccountService)
        {
            _websiteAccountService = websiteAccountService;
        }
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _websiteAccountService.GetAll();
            return Response(result);
        }

        [HttpGet("Get-By-Id")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            try
            {
                var result = await _websiteAccountService.GetById(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
