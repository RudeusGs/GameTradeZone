using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.Authenticate;
using Microsoft.AspNetCore.Mvc;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : BaseController
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthenticateController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel request)
        {
            try
            {
                var result = await _authenticateService.Login(request);
                return Response(result);
            }
            catch (Exception ex)
            {
                return Response(ex.Message, 500);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel request)
        {
            try
            {
                var result = await _authenticateService.Register(request);
                return Response(new JsonResult(result.Data));
            }
            catch (Exception ex)
            {
                return Response(ex.Message, 500);
            }
        }
        [HttpGet("roles")]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            var result = await _authenticateService.GetUserRoles(userId);
            if (result.Data != null)
            {
                return Response(result.Data);
            }
            return Response(result.Message);
        }
    }
}
