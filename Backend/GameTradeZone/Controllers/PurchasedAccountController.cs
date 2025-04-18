using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.PurchasedAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasedAccountController : BaseController
    {
        private readonly IPurchasedAccountService _purchasedAccountService;

        public PurchasedAccountController(IPurchasedAccountService purchasedAccountService)
        {
            _purchasedAccountService = purchasedAccountService;
        }

        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _purchasedAccountService.GetAll();
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
                var result = await _purchasedAccountService.GetAllByUserID(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [Authorize]
        [HttpGet("Get-Dont-Confirm")]
        public async Task<IActionResult> GetDontConfirm()
        {
            try
            {
                var result = await _purchasedAccountService.GetDontConfirm();
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
                var result = await _purchasedAccountService.Delete(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Confirm-Account")]
        public async Task<IActionResult> ConfirmAccount(ComfirmModel model)
        {
            try
            {
                var result = await _purchasedAccountService.ComfirmAccount(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Email-request")]
        public async Task<IActionResult> EmailRequest(int id)
        {
            try
            {
                var result = await _purchasedAccountService.EmailRequest(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("Email-response")]
        public async Task<IActionResult> EmailResponse(int id, string model)
        {
            try
            {
                var result = await _purchasedAccountService.EmailResponse(id, model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("OTP-request")]
        public async Task<IActionResult> OTPRequest(int id)
        {
            try
            {
                var result = await _purchasedAccountService.OTPRequest(id);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpPost("OTP-response")]
        public async Task<IActionResult> OTPResponse(int id, string model)
        {
            try
            {
                var result = await _purchasedAccountService.OTPResponse(id, model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [Authorize]
        [HttpPost("Resend-Email")]
        public async Task<IActionResult> ResendEmail(int id, string model)
        {
            try
            {
                var result = await _purchasedAccountService.ResendEmail(id, model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [Authorize]
        [HttpPost("Resend-OTP")]
        public async Task<IActionResult> ResendOTP(int id, string model)
        {
            try
            {
                var result = await _purchasedAccountService.ResendOTP(id, model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}