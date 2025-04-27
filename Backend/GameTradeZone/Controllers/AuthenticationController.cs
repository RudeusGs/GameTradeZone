using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models.Authenticate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : BaseController
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly SignInManager<User> _signInManager;

        public AuthenticateController(
            IAuthenticateService authenticateService,
            SignInManager<User> signInManager)
        {
            _authenticateService = authenticateService;
            _signInManager = signInManager;
        }
        [HttpPost("send-otp-email-verification")]
        [Authorize]
        public async Task<IActionResult> SendOtpForEmailVerification()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var result = await _authenticateService.SendOtpForEmailVerificationAsync(email);
            if (result.Data != null)
                return Ok(result.Data);
            return BadRequest(result.Message);
        }
        [HttpPost("verify-otp-email-verification")]
        [Authorize]
        public async Task<IActionResult> VerifyOtpForEmailVerification([FromBody] VerifyOtpModel model)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            model.UserName = userName;
            var result = await _authenticateService.VerifyOtpForEmailVerificationAsync(model);
            if (result.Data != null)
                return Ok(result.Data);
            return BadRequest(result.Message);
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
        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider = "Google", string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Authenticate", new { returnUrl }, Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
                return Redirect($"http://localhost:5173/callback?error={Uri.EscapeDataString(remoteError)}");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Redirect("http://localhost:5173/callback?error=Không thể lấy thông tin đăng nhập từ Google.");

            var result = await _authenticateService.ExternalLoginAsync(info);
            if (result.Data != null)
            {
                var token = result.Data.GetType().GetProperty("Token")?.GetValue(result.Data)?.ToString();
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return Redirect($"http://localhost:5173/callback?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email ?? "")}");
            }
            return Redirect($"http://localhost:5173/callback?error={Uri.EscapeDataString(result.Message ?? "Lỗi không xác định")}");
        }
        [HttpPut("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _authenticateService.UpdateUser(userId, model);
            if (result.Data != null)
                return Ok(result.Data);
            return BadRequest(result.Message);
        }
        [HttpPost("send-custom-email")]
        [Authorize]
        public async Task<IActionResult> SendCustomEmail(SendCustomEmailRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }

            var result = await _authenticateService.SendCustomEmailAsync(model.UserId, model.Subject, model.MessageBody);
            if (result.Data != null)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }
        public class SendCustomEmailRequest
        {
            public int UserId { get; set; }
            public string Subject { get; set; }
            public string MessageBody { get; set; }
        }
    }
}
