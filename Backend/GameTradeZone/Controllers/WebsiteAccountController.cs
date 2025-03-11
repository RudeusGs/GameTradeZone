using GameTradeZone.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GameTradeZone.Service.Models;
using Microsoft.AspNetCore.Authorization;

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

        [HttpPost("Block-Account")]
        public async Task<IActionResult> BlockAccount([FromQuery] int id)
        {
            try
            {
                var result = await _websiteAccountService.BlockAccount(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [HttpPost("Update-Roles")]
        public async Task<IActionResult> UpdateRoles([FromQuery] int id, [FromBody] List<string> newRoles)
        {
            try
            {
                var result = await _websiteAccountService.UpdateRoles(id, newRoles);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [Authorize]
        [HttpPost("Update-User-Avatar")]
        public async Task<IActionResult> UpdateUserAvatar([FromForm] IFormFile image)
        {
            try
            {
                var result = await _websiteAccountService.UpdateUserAvatar(image);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}